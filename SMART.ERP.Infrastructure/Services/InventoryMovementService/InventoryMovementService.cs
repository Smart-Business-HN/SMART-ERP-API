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
            return movement;
        }

        private async Task SyncStockAsync(RecordMovementInput input, decimal newAvg, CancellationToken cancellationToken)
        {
            // Local-first: reusar instancias ya rastreadas para evitar conflictos cuando el mismo
            // producto aparece en varias líneas de un mismo documento dentro de la transacción.
            var distributions = await _context.InventoryDistributions
                .Include(x => x.Warehouse)
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
            bool newDistributionIsVirtualWarehouse = false;

            if (distribution == null)
            {
                // Proyectar solo IsVirtual: cargar la entidad Warehouse y asignarla al navigation
                // property haría que EF la trate como Added (Detached + Id != 0 por NoTracking global)
                // e intente INSERT INTO [Warehouse] ([Id], ...) con IDENTITY_INSERT OFF.
                newDistributionIsVirtualWarehouse = await _context.Warehouses
                    .Where(w => w.Id == input.WarehouseId)
                    .Select(w => w.IsVirtual)
                    .FirstOrDefaultAsync(cancellationToken);

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

            var product = _context.Products.Local.FirstOrDefault(p => p.Id == input.ProductId);
            if (product == null)
            {
                product = await _context.Products.FirstOrDefaultAsync(x => x.Id == input.ProductId, cancellationToken);
                if (product != null) _context.Products.Attach(product);
            }
            if (product != null)
            {
                // Solo el stock propio cuenta como CurrentStock; los almacenes virtuales (consignados)
                // se excluyen para no inflar el inventario propio ni afectar reportes/contabilidad.
                // Las distribuciones existentes traen Warehouse via .Include; la recién creada
                // tiene Warehouse == null y usamos newDistributionIsVirtualWarehouse para su flag.
                product.CurrentStock = (int)distributions
                    .Where(x =>
                        (x.Warehouse != null && !x.Warehouse.IsVirtual)
                        || (x.Warehouse == null && !newDistributionIsVirtualWarehouse))
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
            var distributions = await _context.InventoryDistributions
                .Include(x => x.Warehouse)
                .Where(x => x.ProductId == productId)
                .ToListAsync(cancellationToken);

            for (int i = 0; i < distributions.Count; i++)
            {
                var tracked = _context.InventoryDistributions.Local.FirstOrDefault(d => d.Id == distributions[i].Id);
                if (tracked != null) distributions[i] = tracked;
            }

            // Excluir almacenes virtuales: el stock consignado no forma parte del inventario propio.
            var total = distributions
                .Where(x => x.Warehouse == null || !x.Warehouse.IsVirtual)
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
