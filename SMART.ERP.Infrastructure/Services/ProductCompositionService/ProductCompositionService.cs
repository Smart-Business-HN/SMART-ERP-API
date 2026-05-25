using Microsoft.EntityFrameworkCore;
using SMART.ERP.Application.Services.ProductCompositionService;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Infrastructure.Services.ProductCompositionService
{
    public class ProductCompositionService : IProductCompositionService
    {
        private readonly DatabaseContext _context;

        public ProductCompositionService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<ComponentMovement>> ResolveComponentsForSaleAsync(int productId, decimal saleQuantity, CancellationToken cancellationToken = default)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Where(x => x.Id == productId)
                .Select(x => new { x.Id, x.ProductType, x.CostPrice })
                .FirstOrDefaultAsync(cancellationToken);

            if (product == null) return new List<ComponentMovement>();

            if (product.ProductType == ProductType.Service)
                return new List<ComponentMovement>();

            if (product.ProductType == ProductType.Combo)
            {
                var parts = await (from p in _context.ProductParts.AsNoTracking()
                                   join c in _context.Products.AsNoTracking() on p.ProductId equals c.Id
                                   where p.FatherProductId == productId && p.IsActive
                                   select new { p.ProductId, p.Quantity, c.CostPrice })
                                  .ToListAsync(cancellationToken);

                return parts.Select(p => new ComponentMovement
                {
                    ComponentProductId = p.ProductId,
                    QuantityOut = saleQuantity * p.Quantity,
                    UnitCost = p.CostPrice
                }).ToList();
            }

            return new List<ComponentMovement>
            {
                new ComponentMovement
                {
                    ComponentProductId = product.Id,
                    QuantityOut = saleQuantity,
                    UnitCost = product.CostPrice
                }
            };
        }

        public async Task<decimal> GetCalculatedComboStockAsync(int comboId, CancellationToken cancellationToken = default)
        {
            var parts = await (from p in _context.ProductParts.AsNoTracking()
                               join c in _context.Products.AsNoTracking() on p.ProductId equals c.Id
                               where p.FatherProductId == comboId && p.IsActive
                               select new { p.Quantity, ComponentStock = (decimal)c.CurrentStock })
                              .ToListAsync(cancellationToken);

            if (parts.Count == 0) return 0m;
            if (parts.Any(p => p.Quantity <= 0)) return 0m;

            var min = parts.Min(p => p.ComponentStock / p.Quantity);
            return Math.Floor(min);
        }

        public async Task<Dictionary<int, decimal>> GetCalculatedStockMapAsync(IEnumerable<int> comboIds, CancellationToken cancellationToken = default)
        {
            var ids = comboIds?.Distinct().ToList() ?? new List<int>();
            if (ids.Count == 0) return new Dictionary<int, decimal>();

            var rows = await (from p in _context.ProductParts.AsNoTracking()
                              join c in _context.Products.AsNoTracking() on p.ProductId equals c.Id
                              where ids.Contains(p.FatherProductId) && p.IsActive
                              select new { p.FatherProductId, p.Quantity, ComponentStock = (decimal)c.CurrentStock })
                             .ToListAsync(cancellationToken);

            var result = new Dictionary<int, decimal>();
            foreach (var id in ids)
            {
                var combo = rows.Where(r => r.FatherProductId == id).ToList();
                if (combo.Count == 0 || combo.Any(p => p.Quantity <= 0))
                {
                    result[id] = 0m;
                    continue;
                }
                result[id] = Math.Floor(combo.Min(p => p.ComponentStock / p.Quantity));
            }
            return result;
        }
    }
}
