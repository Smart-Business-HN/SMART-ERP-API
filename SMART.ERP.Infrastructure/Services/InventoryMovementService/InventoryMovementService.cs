using Microsoft.EntityFrameworkCore;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Services.InventoryMovementService;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Infrastructure.Services.InventoryMovementService
{
    public class InventoryMovementService : IInventoryMovementService
    {
        private readonly DatabaseContext _context;

        public InventoryMovementService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<InventoryMovement> RecordMovementAsync(RecordMovementInput input, CancellationToken cancellationToken = default)
        {
            if (input.QuantityIn < 0 || input.QuantityOut < 0)
                throw new ApiException("Las cantidades del movimiento no pueden ser negativas.");
            if (input.QuantityIn == 0 && input.QuantityOut == 0)
                throw new ApiException("El movimiento debe tener una cantidad de entrada o de salida.");

            var last = await _context.InventoryMovements
                .AsNoTracking()
                .Where(x => x.ProductId == input.ProductId && x.WarehouseId == input.WarehouseId)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            decimal prevQty = last?.RunningQuantity ?? 0m;
            decimal prevAvg = last?.RunningAverageCost ?? 0m;

            // Inserción retro-fechada (p.ej. un inventario inicial con fecha anterior a ventas ya
            // registradas): los saldos corridos quedarían fuera de orden, así que al final se recalcula
            // toda la cadena en orden cronológico. Solo ocurre en este caso atípico.
            var isBackdated = last != null && input.MovementDate < last.MovementDate;

            decimal newQty;
            decimal newAvg;

            if (input.QuantityIn > 0)
            {
                newQty = prevQty + input.QuantityIn;
                if (!input.IsCancellation && input.UnitCost.HasValue && input.UnitCost.Value > 0)
                {
                    var baseQty = prevQty > 0 ? prevQty : 0m;
                    if (baseQty <= 0)
                    {
                        newAvg = input.UnitCost.Value;
                    }
                    else
                    {
                        var totalValuePrev = baseQty * prevAvg;
                        var totalValueIn = input.QuantityIn * input.UnitCost.Value;
                        newAvg = Math.Round((totalValuePrev + totalValueIn) / (baseQty + input.QuantityIn), 4);
                    }
                }
                else
                {
                    newAvg = prevAvg;
                }
            }
            else
            {
                newQty = prevQty - input.QuantityOut;
                newAvg = prevAvg;
            }

            var movement = new InventoryMovement
            {
                ProductId = input.ProductId,
                WarehouseId = input.WarehouseId,
                MovementDate = input.MovementDate,
                MovementType = input.MovementType,
                DocumentType = input.DocumentType,
                DocumentId = input.DocumentId,
                DocumentCode = input.DocumentCode,
                ThirdPartyName = input.ThirdPartyName,
                QuantityIn = input.QuantityIn,
                QuantityOut = input.QuantityOut,
                UnitCost = input.UnitCost,
                TotalCost = input.UnitCost.HasValue
                    ? Math.Round((input.QuantityIn + input.QuantityOut) * input.UnitCost.Value, 2)
                    : null,
                RunningQuantity = newQty,
                RunningAverageCost = newAvg,
                RunningTotalValue = Math.Round(newQty * newAvg, 2),
                UserId = input.UserId,
                UserName = input.UserName,
                Notes = input.Notes,
                IsCancellation = input.IsCancellation,
                CancelledMovementId = input.CancelledMovementId,
                CreationDate = DateTime.Now
            };

            await _context.InventoryMovements.AddAsync(movement, cancellationToken);

            if (input.SyncStock)
            {
                await SyncStockAsync(input, newAvg, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Si el movimiento entró con fecha anterior a otros ya registrados, recalcular la cadena
            // completa en orden cronológico para que el kardex (saldo, costo promedio y valor) cuadre.
            if (isBackdated)
                await RecalculateRunningBalancesAsync(input.ProductId, input.WarehouseId, input.SyncStock, cancellationToken);

            return movement;
        }

        /// <summary>
        /// Recalcula los saldos corridos (RunningQuantity / RunningAverageCost / RunningTotalValue) de
        /// todos los movimientos de un producto en un almacén, en orden cronológico (fecha, luego Id).
        /// Necesario cuando se inserta un movimiento retro-fechado (p.ej. un inventario inicial) y la
        /// cadena quedó fuera de orden. NO altera QuantityIn/Out/UnitCost/TotalCost: el costo registrado
        /// de cada transacción se conserva; solo se corrigen los saldos derivados. Replica el mismo
        /// algoritmo de costo promedio que <see cref="RecordMovementAsync"/>.
        /// </summary>
        private async Task RecalculateRunningBalancesAsync(int productId, int warehouseId, bool updateProductCost, CancellationToken cancellationToken)
        {
            var movements = await _context.InventoryMovements
                .Where(x => x.ProductId == productId && x.WarehouseId == warehouseId)
                .OrderBy(x => x.MovementDate)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
            if (movements.Count == 0) return;

            // Reusar instancias ya rastreadas (Local) para no chocar con el ChangeTracker.
            for (int i = 0; i < movements.Count; i++)
            {
                var tracked = _context.InventoryMovements.Local.FirstOrDefault(m => m.Id == movements[i].Id);
                if (tracked != null) movements[i] = tracked;
            }

            decimal runQty = 0m, runAvg = 0m;
            foreach (var m in movements)
            {
                if (m.QuantityIn > 0)
                {
                    if (!m.IsCancellation && m.UnitCost.HasValue && m.UnitCost.Value > 0)
                    {
                        if (runQty <= 0)
                            runAvg = m.UnitCost.Value;
                        else
                            runAvg = Math.Round((runQty * runAvg + m.QuantityIn * m.UnitCost.Value) / (runQty + m.QuantityIn), 4);
                    }
                    runQty += m.QuantityIn;
                }
                else
                {
                    runQty -= m.QuantityOut;
                }

                m.RunningQuantity = runQty;
                m.RunningAverageCost = runAvg;
                m.RunningTotalValue = Math.Round(runQty * runAvg, 2);
                if (_context.Entry(m).State == EntityState.Detached)
                    _context.InventoryMovements.Update(m);
            }

            // El costo del producto refleja el promedio del último movimiento cronológico.
            if (updateProductCost && runAvg > 0)
            {
                var product = _context.Products.Local.FirstOrDefault(p => p.Id == productId);
                if (product == null)
                {
                    product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId, cancellationToken);
                    if (product != null) _context.Products.Attach(product);
                }
                if (product != null) product.CostPrice = runAvg;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SyncStockAsync(RecordMovementInput input, decimal newAvg, CancellationToken cancellationToken)
        {
            // Local-first: reusar instancias ya rastreadas para evitar conflictos cuando el mismo
            // producto aparece en varias líneas de un mismo documento dentro de la transacción.
            // NO se hace .Include(x => x.Warehouse): adjuntar la distribución vía .Update() arrastraría
            // la entidad Warehouse al ChangeTracker y, al procesar varios items del mismo almacén en una
            // sola transacción, EF intentaría rastrear dos instancias de Warehouse con el mismo Id
            // ("another instance with the same key value is already being tracked").
            var distributions = await _context.InventoryDistributions
                .Where(x => x.ProductId == input.ProductId)
                .ToListAsync(cancellationToken);

            // Sustituir por las instancias rastreadas (Local) cuando existan.
            for (int i = 0; i < distributions.Count; i++)
            {
                var tracked = _context.InventoryDistributions.Local.FirstOrDefault(d => d.Id == distributions[i].Id);
                if (tracked != null) distributions[i] = tracked;
            }

            var distribution = distributions.FirstOrDefault(x => x.WarehouseId == input.WarehouseId);
            var delta = input.QuantityIn - input.QuantityOut;

            if (distribution == null)
            {
                distribution = new InventoryDistribution
                {
                    ProductId = input.ProductId,
                    WarehouseId = input.WarehouseId,
                    Quantity = delta,
                    CreationDate = DateTime.Now,
                    CreatedBy = input.UserName
                };
                await _context.InventoryDistributions.AddAsync(distribution, cancellationToken);
                distributions.Add(distribution);
            }
            else
            {
                distribution.Quantity += delta;
                distribution.ModificationDate = DateTime.Now;
                distribution.ModifiedBy = input.UserName;
                if (_context.Entry(distribution).State == EntityState.Detached)
                    _context.InventoryDistributions.Update(distribution);
            }

            // Solo el stock propio cuenta como CurrentStock; los almacenes virtuales (consignados) se
            // excluyen para no inflar el inventario propio ni afectar reportes/contabilidad. Se resuelve
            // qué almacenes son virtuales con una proyección de Ids (sin materializar Warehouse).
            var warehouseIds = distributions.Select(x => x.WarehouseId).Distinct().ToList();
            var virtualWarehouseIds = (await _context.Warehouses
                .Where(w => warehouseIds.Contains(w.Id) && w.IsVirtual)
                .Select(w => w.Id)
                .ToListAsync(cancellationToken)).ToHashSet();

            var product = _context.Products.Local.FirstOrDefault(p => p.Id == input.ProductId);
            if (product == null)
            {
                product = await _context.Products.FirstOrDefaultAsync(x => x.Id == input.ProductId, cancellationToken);
                if (product != null) _context.Products.Attach(product);
            }
            if (product != null)
            {
                product.CurrentStock = (int)distributions
                    .Where(x => !virtualWarehouseIds.Contains(x.WarehouseId))
                    .Sum(x => x.Quantity);
                if (input.UpdateProductCost && !input.IsCancellation && input.QuantityIn > 0 && input.UnitCost.HasValue && input.UnitCost.Value > 0)
                {
                    product.CostPrice = newAvg;
                }
            }
        }

        public async Task RecordCancellationForDocumentAsync(string documentType, int documentId, DateTime movementDate, KardexMovementType cancellationType, Guid? userId, string? userName, CancellationToken cancellationToken = default)
        {
            var movements = await _context.InventoryMovements
                .AsNoTracking()
                .Where(x => x.DocumentType == documentType && x.DocumentId == documentId && !x.IsCancellation)
                .ToListAsync(cancellationToken);

            foreach (var original in movements)
            {
                var reverseInput = new RecordMovementInput
                {
                    ProductId = original.ProductId,
                    WarehouseId = original.WarehouseId,
                    MovementDate = movementDate,
                    MovementType = cancellationType,
                    DocumentType = documentType,
                    DocumentId = documentId,
                    DocumentCode = original.DocumentCode,
                    ThirdPartyName = original.ThirdPartyName,
                    QuantityIn = original.QuantityOut,
                    QuantityOut = original.QuantityIn,
                    UnitCost = original.UnitCost,
                    UserId = userId,
                    UserName = userName,
                    Notes = "Reverso por cancelación",
                    SyncStock = true,
                    UpdateProductCost = false,
                    IsCancellation = true,
                    CancelledMovementId = original.Id
                };
                await RecordMovementAsync(reverseInput, cancellationToken);
            }
        }

        public async Task SyncProductStockAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Sin .Include(x => x.Warehouse): evita rastrear la entidad Warehouse (ver nota en SyncStockAsync).
            var distributions = await _context.InventoryDistributions
                .Where(x => x.ProductId == productId)
                .ToListAsync(cancellationToken);

            for (int i = 0; i < distributions.Count; i++)
            {
                var tracked = _context.InventoryDistributions.Local.FirstOrDefault(d => d.Id == distributions[i].Id);
                if (tracked != null) distributions[i] = tracked;
            }

            // Excluir almacenes virtuales: el stock consignado no forma parte del inventario propio.
            // Se resuelve con una proyección de Ids (sin materializar la entidad Warehouse).
            var warehouseIds = distributions.Select(x => x.WarehouseId).Distinct().ToList();
            var virtualWarehouseIds = (await _context.Warehouses
                .Where(w => warehouseIds.Contains(w.Id) && w.IsVirtual)
                .Select(w => w.Id)
                .ToListAsync(cancellationToken)).ToHashSet();

            var total = distributions
                .Where(x => !virtualWarehouseIds.Contains(x.WarehouseId))
                .Sum(x => x.Quantity);

            var product = _context.Products.Local.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId, cancellationToken);
                if (product != null) _context.Products.Attach(product);
            }
            if (product != null)
            {
                product.CurrentStock = (int)total;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public KardexMovementType MapExitReasonToMovementType(InventoryExitReason reason)
        {
            return reason switch
            {
                InventoryExitReason.Shrinkage => KardexMovementType.Shrinkage,
                InventoryExitReason.Sample => KardexMovementType.Sample,
                InventoryExitReason.Gift => KardexMovementType.Gift,
                InventoryExitReason.Damage => KardexMovementType.Damage,
                InventoryExitReason.Theft => KardexMovementType.Theft,
                InventoryExitReason.InternalUse => KardexMovementType.InternalUse,
                InventoryExitReason.Expiration => KardexMovementType.Expiration,
                _ => KardexMovementType.OtherExit
            };
        }
    }
}
