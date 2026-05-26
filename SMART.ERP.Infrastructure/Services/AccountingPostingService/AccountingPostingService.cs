using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Services.AccountingPostingService;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Infrastructure.Services.AccountingPostingService
{
    /// <summary>
    /// Implementación del posteo automático de asientos contables desde documentos operativos.
    /// Comparte el DatabaseContext (scoped) con los handlers, por lo que participa en sus transacciones.
    /// Modelo de dimensiones (NIIF para PYMES): Cliente/Proveedor van como Tercero (CustomerId/ProviderId)
    /// en la línea, no como cuentas auxiliares. La línea de negocio se etiqueta vía CostCenterId.
    /// </summary>
    public class AccountingPostingService : IAccountingPostingService
    {
        private const string DefaultCostCenterCode = "99"; // 99 General / Administración
        private readonly DatabaseContext _context;
        private readonly IJwtService _jwtService;

        // Cache por instancia (scoped = una por request).
        private int? _cachedDefaultCostCenterId;

        public AccountingPostingService(DatabaseContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        private sealed class Line
        {
            public int LedgerAccountId { get; init; }
            public decimal Debit { get; init; }
            public decimal Credit { get; init; }
            public string? Description { get; init; }
            public int? CostCenterId { get; init; }
            public Guid? CustomerId { get; init; }
            public int? ProviderId { get; init; }
        }

        // ============================ VENTAS ============================

        public async Task PostInvoiceAsync(int invoiceId, CancellationToken ct)
        {
            if (!await IsEnabledAsync(ct)) return;

            var invoice = await _context.Invoices.AsNoTracking().FirstOrDefaultAsync(x => x.Id == invoiceId, ct);
            if (invoice == null) return;

            var costCenter = await DefaultCostCenterIdAsync(ct);
            var net = Round(invoice.Total - invoice.Taxes15Percent - invoice.Taxes18Percent);
            var ar = await SystemAccountAsync(AccountMappingKey.AccountsReceivable, ct);
            // Si la factura es una suscripción prepagada (SaaS), el ingreso no se reconoce de inmediato:
            // el neto va a 2108001 Ingresos Diferidos (pasivo) y se devenga 1/N mensual contra 4103001 vía RevenueRecognitionJob.
            var creditAccountKey = invoice.IsDeferredRevenue ? AccountMappingKey.DeferredRevenueSaaS : AccountMappingKey.SalesRevenue;
            var creditAccount = await SystemAccountAsync(creditAccountKey, ct);
            var creditDescription = invoice.IsDeferredRevenue ? "Ingresos diferidos (suscripción)" : "Ingreso por ventas";

            var lines = new List<Line>
            {
                new() { LedgerAccountId = ar, Debit = Round(invoice.Total), Description = "Cuentas por cobrar", CustomerId = invoice.CustomerId },
                // En facturas diferidas no se etiqueta CostCenter en la línea del pasivo (NIIF: el pasivo no es de resultados).
                new() { LedgerAccountId = creditAccount, Credit = net, Description = creditDescription, CustomerId = invoice.CustomerId, CostCenterId = invoice.IsDeferredRevenue ? null : costCenter },
            };
            if (invoice.Taxes15Percent > 0)
                lines.Add(new() { LedgerAccountId = await SystemAccountAsync(AccountMappingKey.SalesTaxPayable15, ct), Credit = Round(invoice.Taxes15Percent), Description = "ISV 15% débito fiscal", CustomerId = invoice.CustomerId });
            if (invoice.Taxes18Percent > 0)
                lines.Add(new() { LedgerAccountId = await SystemAccountAsync(AccountMappingKey.SalesTaxPayable18, ct), Credit = Round(invoice.Taxes18Percent), Description = "ISV 18% débito fiscal", CustomerId = invoice.CustomerId });

            // COGS desde el kardex real de la factura.
            var cogs = await _context.InventoryMovements.AsNoTracking()
                .Where(m => m.DocumentType == "Invoice" && m.DocumentId == invoiceId
                            && (m.MovementType == KardexMovementType.Sale || m.MovementType == KardexMovementType.ManualSale))
                .Select(m => m.TotalCost ?? (m.QuantityOut * (m.UnitCost ?? 0m)))
                .SumAsync(ct);
            cogs = Round(cogs);
            if (cogs > 0)
            {
                lines.Add(new() { LedgerAccountId = await SystemAccountAsync(AccountMappingKey.CostOfGoodsSold, ct), Debit = cogs, Description = "Costo de venta", CostCenterId = costCenter });
                lines.Add(new() { LedgerAccountId = await SystemAccountAsync(AccountMappingKey.Inventory, ct), Credit = cogs, Description = "Inventario" });
            }

            await PostEntryAsync(invoice.CreationDate, $"Venta factura {invoice.InvoiceNumber}", JournalEntrySource.Invoice, "Invoice", invoice.Id, lines, ct);
        }

        /// <summary>
        /// Devengo mensual de una factura SaaS prepagada: reclasifica 1/N del neto desde 2108001
        /// Ingresos diferidos hacia 4103001 Ingreso SaaS, y actualiza MonthsRecognized/NextRecognitionDate.
        /// Idempotente por mes: cada reconocimiento se identifica con SourceDocumentType="InvoiceRecognition",
        /// SourceDocumentId=invoiceId, Reference="M{monthIndex}". Se cancela cuando NextRecognitionDate &gt; hoy.
        /// </summary>
        public async Task<bool> PostInvoiceRevenueRecognitionAsync(int invoiceId, CancellationToken ct)
        {
            if (!await IsEnabledAsync(ct)) return false;

            // Cargamos la factura con tracking porque vamos a actualizar contadores.
            var invoice = await _context.Invoices.FirstOrDefaultAsync(x => x.Id == invoiceId, ct);
            if (invoice == null || !invoice.IsDeferredRevenue || invoice.RecognitionMonths is not int totalMonths || totalMonths <= 0)
                return false;
            if (invoice.MonthsRecognized >= totalMonths || invoice.NextRecognitionDate == null) return false;
            if (invoice.NextRecognitionDate.Value.Date > DateTime.Now.Date) return false; // todavía no toca

            var monthIndex = invoice.MonthsRecognized + 1;
            var reference = $"M{monthIndex}";

            // Si por alguna razón ya existe el asiento (carrera o re-disparo), no duplicar.
            var already = await _context.JournalEntries.AsNoTracking().AnyAsync(
                x => x.SourceDocumentType == "InvoiceRecognition" && x.SourceDocumentId == invoiceId
                     && x.Reference == reference
                     && (x.Status == JournalEntryStatus.Posted || x.Status == JournalEntryStatus.ReversalEntry), ct);
            if (already)
            {
                // Avanzamos contadores de la factura para que la próxima ejecución no vuelva al mismo mes.
                AdvanceRecognition(invoice, totalMonths);
                await _context.SaveChangesAsync(ct);
                return false;
            }

            // Importe neto a devengar = Total factura sin ISV.
            var net = Round(invoice.Total - invoice.Taxes15Percent - invoice.Taxes18Percent);
            if (net <= 0) return false;
            // Fraccionamiento N-1 meses iguales + último mes ajusta el residuo para sumar exacto.
            var perMonth = Round(net / totalMonths);
            var amount = monthIndex < totalMonths ? perMonth : Round(net - perMonth * (totalMonths - 1));
            if (amount <= 0) return false;

            var costCenter = await DefaultCostCenterIdAsync(ct);
            var deferred = await SystemAccountAsync(AccountMappingKey.DeferredRevenueSaaS, ct);
            var saas = await SystemAccountAsync(AccountMappingKey.SaaSRevenue, ct);
            var recognitionDate = invoice.NextRecognitionDate.Value;

            var lines = new List<Line>
            {
                new() { LedgerAccountId = deferred, Debit = amount, Description = $"Devengo mes {monthIndex}/{totalMonths} factura {invoice.InvoiceNumber}", CustomerId = invoice.CustomerId },
                new() { LedgerAccountId = saas, Credit = amount, Description = $"Ingreso SaaS mes {monthIndex}/{totalMonths}", CustomerId = invoice.CustomerId, CostCenterId = costCenter },
            };

            await PostEntryInternalAsync(recognitionDate, $"Devengo SaaS factura {invoice.InvoiceNumber} mes {monthIndex}/{totalMonths}",
                JournalEntrySource.Invoice, "InvoiceRecognition", invoiceId, reference, lines, ct);

            AdvanceRecognition(invoice, totalMonths);
            await _context.SaveChangesAsync(ct);
            return true;
        }

        private static void AdvanceRecognition(Invoice invoice, int totalMonths)
        {
            invoice.MonthsRecognized += 1;
            if (invoice.MonthsRecognized >= totalMonths)
            {
                invoice.NextRecognitionDate = null;
            }
            else if (invoice.NextRecognitionDate.HasValue)
            {
                // Avanza siempre desde la fecha base para mantener el mismo día del mes (evita drift).
                var baseDate = invoice.RecognitionStartDate ?? invoice.NextRecognitionDate.Value;
                invoice.NextRecognitionDate = baseDate.AddMonths(invoice.MonthsRecognized + 1);
            }
        }

        public async Task PostBillPaymentAsync(int billPaymentId, CancellationToken ct)
        {
            if (!await IsEnabledAsync(ct)) return;
            var payment = await _context.BillPayments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == billPaymentId, ct);
            if (payment == null) return;
            var invoice = await _context.Invoices.AsNoTracking().FirstOrDefaultAsync(i => i.Id == payment.InvoiceId, ct)
                ?? throw new ApiException($"No se encontró la factura {payment.InvoiceId} del cobro {billPaymentId}.");

            var ar = await SystemAccountAsync(AccountMappingKey.AccountsReceivable, ct);
            var lines = new List<Line>
            {
                new() { LedgerAccountId = await BankAccountAsync(payment.InternalBankAccountId, ct), Debit = Round(payment.Amount), Description = "Cobro recibido" },
                new() { LedgerAccountId = ar, Credit = Round(payment.Amount), Description = "Cuentas por cobrar", CustomerId = invoice.CustomerId },
            };
            await PostEntryAsync(payment.Date, $"Cobro factura #{payment.InvoiceId}", JournalEntrySource.BillPayment, "BillPayment", payment.Id, lines, ct);
        }

        // ============================ COMPRAS ============================

        public async Task PostPurchaseBillAsync(int purchaseBillId, CancellationToken ct)
        {
            if (!await IsEnabledAsync(ct)) return;
            var bill = await _context.PurchaseBills.AsNoTracking().FirstOrDefaultAsync(x => x.Id == purchaseBillId, ct);
            if (bill == null) return;

            var costCenter = await DefaultCostCenterIdAsync(ct);
            var net = Round(bill.Total - bill.Taxes15Percent - bill.Taxes18Percent);
            var lines = new List<Line>
            {
                new() { LedgerAccountId = await ExpenseAccountAsync(bill.ExpenseAccountId, ct), Debit = net, Description = "Compra / gasto", CostCenterId = costCenter, ProviderId = bill.ProviderId },
            };
            if (bill.Taxes15Percent > 0)
                lines.Add(new() { LedgerAccountId = await SystemAccountAsync(AccountMappingKey.InputTaxCredit15, ct), Debit = Round(bill.Taxes15Percent), Description = "Crédito fiscal ISV 15%", ProviderId = bill.ProviderId });
            if (bill.Taxes18Percent > 0)
                lines.Add(new() { LedgerAccountId = await SystemAccountAsync(AccountMappingKey.InputTaxCredit18, ct), Debit = Round(bill.Taxes18Percent), Description = "Crédito fiscal ISV 18%", ProviderId = bill.ProviderId });
            // CxP se acredita por el neto a pagar (Total - retenido). El monto retenido se cierra contra el pasivo fiscal.
            var withholding = Round(bill.WithholdingAmount);
            lines.Add(new() { LedgerAccountId = await SystemAccountAsync(AccountMappingKey.AccountsPayable, ct), Credit = Round(bill.Total - withholding), Description = "Cuentas por pagar", ProviderId = bill.ProviderId });
            if (withholding > 0)
            {
                var (whKey, whDescription) = ResolveWithholdingMapping(bill.WithholdingType);
                lines.Add(new() { LedgerAccountId = await SystemAccountAsync(whKey, ct), Credit = withholding, Description = whDescription, ProviderId = bill.ProviderId });
            }

            await PostEntryAsync(bill.InvoiceDate, $"Compra {bill.PurchaseBillCode}", JournalEntrySource.PurchaseBill, "PurchaseBill", bill.Id, lines, ct);
        }

        private static (AccountMappingKey Key, string Description) ResolveWithholdingMapping(WithholdingType type) => type switch
        {
            WithholdingType.ISR12_5 => (AccountMappingKey.WithholdingISR12_5, "Retención ISR 12.5% honorarios"),
            WithholdingType.ISR1 => (AccountMappingKey.WithholdingISR1, "Retención ISR 1% bienes y servicios"),
            WithholdingType.ISV15 => (AccountMappingKey.WithholdingISV15, "Retención ISV 15% Art. 13"),
            _ => throw new ApiException("Tipo de retención no soportado.")
        };

        public async Task PostPurchaseBillPaymentAsync(int purchaseBillPaymentId, CancellationToken ct)
        {
            if (!await IsEnabledAsync(ct)) return;
            var payment = await _context.Set<PurchaseBillPayment>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == purchaseBillPaymentId, ct);
            if (payment == null) return;
            var bill = await _context.PurchaseBills.AsNoTracking().FirstOrDefaultAsync(b => b.Id == payment.PurchaseBillId, ct)
                ?? throw new ApiException($"No se encontró la compra {payment.PurchaseBillId} del pago {purchaseBillPaymentId}.");

            var ap = await SystemAccountAsync(AccountMappingKey.AccountsPayable, ct);
            var lines = new List<Line>
            {
                new() { LedgerAccountId = ap, Debit = Round(payment.Amount), Description = "Cuentas por pagar", ProviderId = bill.ProviderId },
                new() { LedgerAccountId = await BankAccountAsync(payment.InternalBankAccountId, ct), Credit = Round(payment.Amount), Description = "Pago a proveedor" },
            };
            await PostEntryAsync(payment.Date, $"Pago compra #{payment.PurchaseBillId}", JournalEntrySource.PurchaseBillPayment, "PurchaseBillPayment", payment.Id, lines, ct);
        }

        // ============================ GASTOS ============================

        public async Task PostNonBillableExpenseAsync(int nonBillableExpenseId, CancellationToken ct)
        {
            if (!await IsEnabledAsync(ct)) return;
            var expense = await _context.NonBillableExpenses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == nonBillableExpenseId, ct);
            if (expense == null) return;

            var costCenter = await DefaultCostCenterIdAsync(ct);
            var withholding = Round(expense.WithholdingAmount);
            var ap = await SystemAccountAsync(AccountMappingKey.AccountsPayable, ct);
            var lines = new List<Line>
            {
                new() { LedgerAccountId = await ExpenseAccountAsync(expense.ExpenseAccountId, ct), Debit = Round(expense.Amount), Description = expense.Description, CostCenterId = costCenter, ProviderId = expense.ProviderId },
                new() { LedgerAccountId = ap, Credit = Round(expense.Amount) - withholding, Description = "Cuentas por pagar", ProviderId = expense.ProviderId },
            };
            if (withholding > 0)
            {
                var (whKey, whDescription) = ResolveWithholdingMapping(expense.WithholdingType);
                lines.Add(new() { LedgerAccountId = await SystemAccountAsync(whKey, ct), Credit = withholding, Description = whDescription, ProviderId = expense.ProviderId });
            }
            await PostEntryAsync(expense.Date, $"Gasto {expense.ExpenseCode}", JournalEntrySource.NonBillableExpense, "NonBillableExpense", expense.Id, lines, ct);
        }

        public async Task PostNonBillableExpensePaymentAsync(int nonBillableExpensePaymentId, CancellationToken ct)
        {
            if (!await IsEnabledAsync(ct)) return;
            var payment = await _context.NonBillableExpensePayments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == nonBillableExpensePaymentId, ct);
            if (payment == null) return;
            var expense = await _context.NonBillableExpenses.AsNoTracking().FirstOrDefaultAsync(e => e.Id == payment.NonBillableExpenseId, ct)
                ?? throw new ApiException($"No se encontró el gasto {payment.NonBillableExpenseId} del pago {nonBillableExpensePaymentId}.");

            var ap = await SystemAccountAsync(AccountMappingKey.AccountsPayable, ct);
            var lines = new List<Line>
            {
                new() { LedgerAccountId = ap, Debit = Round(payment.Amount), Description = "Cuentas por pagar", ProviderId = expense.ProviderId },
                new() { LedgerAccountId = await BankAccountAsync(payment.InternalBankAccountId, ct), Credit = Round(payment.Amount), Description = "Pago de gasto" },
            };
            await PostEntryAsync(payment.Date, $"Pago gasto #{payment.NonBillableExpenseId}", JournalEntrySource.NonBillableExpense, "NonBillableExpensePayment", payment.Id, lines, ct);
        }

        // ============================ INVENTARIO ============================

        public async Task PostInventoryEntryAsync(int inventoryEntryId, CancellationToken ct)
        {
            if (!await IsEnabledAsync(ct)) return;
            var entry = await _context.InventoryEntries.AsNoTracking().FirstOrDefaultAsync(x => x.Id == inventoryEntryId, ct);
            if (entry == null || entry.EntryType == InventoryEntryType.Purchase) return; // la compra ya postea

            // Defensa en profundidad: los almacenes virtuales (consignados) no afectan contabilidad.
            var isVirtualWarehouse = await _context.Warehouses.AsNoTracking()
                .Where(w => w.Id == entry.WarehouseId)
                .Select(w => w.IsVirtual)
                .FirstOrDefaultAsync(ct);
            if (isVirtualWarehouse) return;

            var movements = await _context.InventoryMovements.AsNoTracking()
                .Where(m => m.DocumentType == "InventoryEntry" && m.DocumentId == inventoryEntryId)
                .Select(m => new { m.QuantityIn, m.QuantityOut, m.UnitCost, m.TotalCost })
                .ToListAsync(ct);
            var net = Round(movements.Sum(m => (m.TotalCost ?? (m.QuantityIn - m.QuantityOut) * (m.UnitCost ?? 0m))));
            if (net == 0) return;

            var inventory = await SystemAccountAsync(AccountMappingKey.Inventory, ct);
            List<Line> lines;
            if (entry.EntryType == InventoryEntryType.OpeningStock)
            {
                lines = new()
                {
                    new() { LedgerAccountId = inventory, Debit = net, Description = "Inventario inicial" },
                    new() { LedgerAccountId = await SystemAccountAsync(AccountMappingKey.OpeningEquity, ct), Credit = net, Description = "Capital / inventario inicial" },
                };
            }
            else // Adjustment
            {
                var costCenter = await DefaultCostCenterIdAsync(ct);
                if (net > 0)
                    lines = new()
                    {
                        new() { LedgerAccountId = inventory, Debit = net, Description = "Ajuste de inventario (incremento)" },
                        new() { LedgerAccountId = await SystemAccountAsync(AccountMappingKey.InventoryAdjustmentIncrease, ct), Credit = net, Description = "Ajuste por incremento", CostCenterId = costCenter },
                    };
                else
                    lines = new()
                    {
                        new() { LedgerAccountId = await SystemAccountAsync(AccountMappingKey.InventoryAdjustmentDecrease, ct), Debit = -net, Description = "Ajuste por disminución", CostCenterId = costCenter },
                        new() { LedgerAccountId = inventory, Credit = -net, Description = "Ajuste de inventario (disminución)" },
                    };
            }
            await PostEntryAsync(entry.EntryDate, $"Entrada de inventario {entry.Code}", JournalEntrySource.Kardex, "InventoryEntry", entry.Id, lines, ct);
        }

        public async Task PostInventoryExitAsync(int inventoryExitId, CancellationToken ct)
        {
            if (!await IsEnabledAsync(ct)) return;
            var exit = await _context.InventoryExits.AsNoTracking().FirstOrDefaultAsync(x => x.Id == inventoryExitId, ct);
            if (exit == null) return;

            // Defensa en profundidad: los almacenes virtuales (consignados) no afectan contabilidad.
            var isVirtualWarehouse = await _context.Warehouses.AsNoTracking()
                .Where(w => w.Id == exit.WarehouseId)
                .Select(w => w.IsVirtual)
                .FirstOrDefaultAsync(ct);
            if (isVirtualWarehouse) return;

            var cost = await _context.InventoryMovements.AsNoTracking()
                .Where(m => m.DocumentType == "InventoryExit" && m.DocumentId == inventoryExitId)
                .Select(m => m.TotalCost ?? (m.QuantityOut * (m.UnitCost ?? 0m)))
                .SumAsync(ct);
            cost = Round(cost);
            if (cost <= 0) return;

            var costCenter = await DefaultCostCenterIdAsync(ct);
            var lines = new List<Line>
            {
                new() { LedgerAccountId = await SystemAccountAsync(AccountMappingKey.DefaultExpense, ct), Debit = cost, Description = "Salida de inventario (merma/uso)", CostCenterId = costCenter },
                new() { LedgerAccountId = await SystemAccountAsync(AccountMappingKey.Inventory, ct), Credit = cost, Description = "Inventario" },
            };
            await PostEntryAsync(exit.ExitDate, $"Salida de inventario {exit.Code}", JournalEntrySource.Kardex, "InventoryExit", exit.Id, lines, ct);
        }

        // ============================ REVERSA ============================

        public async Task ReverseDocumentPostingAsync(string documentType, int documentId, CancellationToken ct)
        {
            if (!await IsEnabledAsync(ct)) return;

            var original = await _context.JournalEntries.AsTracking()
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.SourceDocumentType == documentType && x.SourceDocumentId == documentId
                                       && x.Status == JournalEntryStatus.Posted, ct);
            if (original == null) return;

            var reversalDate = DateTime.Now;
            var period = await ResolveOpenPeriodAsync(reversalDate, ct);

            var userName = SafeUser();
            var lineNumber = 1;
            var reversal = new JournalEntry
            {
                EntryDate = reversalDate,
                FiscalPeriodId = period.Id,
                Description = $"Reversa automática de {original.EntryNumber}",
                Reference = original.EntryNumber,
                Status = JournalEntryStatus.ReversalEntry,
                Source = original.Source,
                SourceDocumentType = documentType,
                SourceDocumentId = documentId,
                TotalDebit = original.TotalCredit,
                TotalCredit = original.TotalDebit,
                EntryNumber = await GenerateEntryNumberAsync(reversalDate.Year, ct),
                PostedDate = DateTime.Now,
                PostedBy = userName,
                CreationDate = DateTime.Now,
                CreatedBy = userName,
                ReversesJournalEntryId = original.Id,
                Lines = (original.Lines ?? new List<JournalEntryLine>())
                    .OrderBy(l => l.LineNumber)
                    .Select(l => new JournalEntryLine
                    {
                        LedgerAccountId = l.LedgerAccountId,
                        LineNumber = lineNumber++,
                        Debit = l.Credit,
                        Credit = l.Debit,
                        Description = l.Description,
                        ProjectId = l.ProjectId,
                        CostCenterId = l.CostCenterId,
                        CustomerId = l.CustomerId,
                        ProviderId = l.ProviderId
                    }).ToList()
            };
            _context.JournalEntries.Add(reversal);
            await _context.SaveChangesAsync(ct);

            original.Status = JournalEntryStatus.Reversed;
            original.ReversedByJournalEntryId = reversal.Id;
            original.ModificationDate = DateTime.Now;
            original.ModifiedBy = userName;
            await _context.SaveChangesAsync(ct);
        }

        // ============================ INTERNOS ============================

        private async Task<bool> IsEnabledAsync(CancellationToken ct)
        {
            var settings = await _context.AccountingSettings.AsNoTracking().FirstOrDefaultAsync(ct);
            return settings?.AutoPostingEnabled ?? false;
        }

        private async Task<int> SystemAccountAsync(AccountMappingKey key, CancellationToken ct)
        {
            var mapping = await _context.AccountingMappings.AsNoTracking().FirstOrDefaultAsync(x => x.Key == key, ct);
            if (mapping?.LedgerAccountId == null)
                throw new ApiException($"Falta configurar la cuenta contable de sistema '{key}'. Configúrela en Configuración Contable.");
            return await EnsurePostableAsync(mapping.LedgerAccountId.Value, key.ToString(), ct);
        }

        private async Task<int> BankAccountAsync(int? internalBankAccountId, CancellationToken ct)
        {
            if (internalBankAccountId.HasValue)
            {
                var bank = await _context.InternalBankAccounts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == internalBankAccountId.Value, ct);
                if (bank?.LedgerAccountId != null)
                    return await EnsurePostableAsync(bank.LedgerAccountId.Value, bank.Name, ct);
            }
            return await SystemAccountAsync(AccountMappingKey.CashOnHand, ct);
        }

        private async Task<int> ExpenseAccountAsync(int legacyExpenseAccountId, CancellationToken ct)
        {
            var ledger = await _context.LedgerAccounts.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ExpenseAccountId == legacyExpenseAccountId && x.IsPostable && x.IsActive, ct);
            if (ledger != null) return ledger.Id;
            return await SystemAccountAsync(AccountMappingKey.DefaultExpense, ct);
        }

        private async Task<int> EnsurePostableAsync(int ledgerAccountId, string context, CancellationToken ct)
        {
            var account = await _context.LedgerAccounts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == ledgerAccountId, ct)
                ?? throw new ApiException($"La cuenta contable mapeada para '{context}' no existe.");
            if (!account.IsPostable || !account.IsActive)
                throw new ApiException($"La cuenta {account.Code} - {account.Name} (mapeo '{context}') no es imputable o está inactiva.");
            return account.Id;
        }

        /// <summary>Centro de costo por defecto (código "99" — General/Administración). Lo carga del seed.</summary>
        private async Task<int?> DefaultCostCenterIdAsync(CancellationToken ct)
        {
            if (_cachedDefaultCostCenterId.HasValue) return _cachedDefaultCostCenterId;
            var cc = await _context.CostCenters.AsNoTracking().FirstOrDefaultAsync(x => x.Code == DefaultCostCenterCode && x.IsActive, ct);
            _cachedDefaultCostCenterId = cc?.Id;
            return _cachedDefaultCostCenterId;
        }

        private Task PostEntryAsync(DateTime date, string description, JournalEntrySource source,
            string docType, int docId, List<Line> lines, CancellationToken ct)
            => PostEntryInternalAsync(date, description, source, docType, docId, reference: null, lines, ct);

        private async Task PostEntryInternalAsync(DateTime date, string description, JournalEntrySource source,
            string docType, int docId, string? reference, List<Line> lines, CancellationToken ct)
        {
            // Idempotencia: para devengo mensual (reference != null) se evalúa también por mes.
            var dupeQuery = _context.JournalEntries.AsNoTracking()
                .Where(x => x.SourceDocumentType == docType && x.SourceDocumentId == docId
                            && (x.Status == JournalEntryStatus.Posted || x.Status == JournalEntryStatus.ReversalEntry));
            if (reference != null) dupeQuery = dupeQuery.Where(x => x.Reference == reference);
            var already = await dupeQuery.AnyAsync(ct);
            if (already) return;

            var totalDebit = Round(lines.Sum(l => l.Debit));
            var totalCredit = Round(lines.Sum(l => l.Credit));
            if (totalDebit <= 0)
                throw new ApiException("El asiento automático no tiene importe (total en cero).");
            if (Math.Abs(totalDebit - totalCredit) > 0.01m)
                throw new ApiException($"El asiento automático no cuadra (Debe {totalDebit:N2} ≠ Haber {totalCredit:N2}).");

            await ValidateLineDimensionsAsync(lines, ct);

            var period = await ResolveOpenPeriodAsync(date, ct);
            var userName = SafeUser();
            var lineNumber = 1;

            var entry = new JournalEntry
            {
                EntryDate = date,
                FiscalPeriodId = period.Id,
                Description = description,
                Reference = reference,
                Status = JournalEntryStatus.Posted,
                Source = source,
                SourceDocumentType = docType,
                SourceDocumentId = docId,
                TotalDebit = totalDebit,
                TotalCredit = totalCredit,
                EntryNumber = await GenerateEntryNumberAsync(date.Year, ct),
                PostedDate = DateTime.Now,
                PostedBy = userName,
                CreationDate = DateTime.Now,
                CreatedBy = userName,
                Lines = lines.Select(l => new JournalEntryLine
                {
                    LedgerAccountId = l.LedgerAccountId,
                    LineNumber = lineNumber++,
                    Debit = l.Debit,
                    Credit = l.Credit,
                    Description = l.Description,
                    CostCenterId = l.CostCenterId,
                    CustomerId = l.CustomerId,
                    ProviderId = l.ProviderId
                }).ToList()
            };
            _context.JournalEntries.Add(entry);
            await _context.SaveChangesAsync(ct);
        }

        /// <summary>Verifica que cada línea cumpla RequiresCostCenter / RequiresTercero de su cuenta.</summary>
        private async Task ValidateLineDimensionsAsync(List<Line> lines, CancellationToken ct)
        {
            var accountIds = lines.Select(l => l.LedgerAccountId).Distinct().ToList();
            var accounts = await _context.LedgerAccounts.AsNoTracking()
                .Where(a => accountIds.Contains(a.Id))
                .Select(a => new { a.Id, a.Code, a.Name, a.RequiresCostCenter, a.RequiresTercero })
                .ToDictionaryAsync(a => a.Id, ct);

            foreach (var line in lines)
            {
                if (!accounts.TryGetValue(line.LedgerAccountId, out var acc)) continue;
                if (acc.RequiresCostCenter && line.CostCenterId == null)
                    throw new ApiException($"La cuenta {acc.Code} - {acc.Name} requiere un Centro de Costo. Asígnelo en el documento o configure el centro de costo por defecto (99 General).");
                if (acc.RequiresTercero && line.CustomerId == null && line.ProviderId == null)
                    throw new ApiException($"La cuenta {acc.Code} - {acc.Name} requiere un Tercero (cliente o proveedor) en la línea.");
            }
        }

        private async Task<FiscalPeriod> ResolveOpenPeriodAsync(DateTime date, CancellationToken ct)
        {
            var day = date.Date;
            var period = await _context.FiscalPeriods.AsNoTracking().FirstOrDefaultAsync(p => p.StartDate <= day && p.EndDate >= day, ct)
                ?? throw new ApiException($"No existe un período fiscal para la fecha {day:dd/MM/yyyy}. Cree el ejercicio fiscal correspondiente.");
            if (period.Status == FiscalPeriodStatus.Closed)
                throw new ApiException($"El período {period.Name} está cerrado; no se puede contabilizar en esa fecha.");
            return period;
        }

        private async Task<string> GenerateEntryNumberAsync(int year, CancellationToken ct)
        {
            var prefix = $"{year}-";
            var last = await _context.JournalEntries.AsNoTracking()
                .Where(x => x.EntryNumber != null && x.EntryNumber.StartsWith(prefix))
                .OrderByDescending(x => x.EntryNumber)
                .FirstOrDefaultAsync(ct);
            var next = 1;
            if (last?.EntryNumber != null)
            {
                var parts = last.EntryNumber.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var seq))
                    next = seq + 1;
            }
            return $"{year}-{next:000000}";
        }

        private string SafeUser()
        {
            try { return _jwtService.GetSubjectToken() ?? "auto"; }
            catch { return "auto"; }
        }

        private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}
