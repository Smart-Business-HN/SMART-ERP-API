using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InventoryDistributionSpecification
{
    public class FilterInventoryByProductWithDetailsSpec : Specification<InventoryDistribution>
    {
        public FilterInventoryByProductWithDetailsSpec(int productId)
        {
            Query.Where(d => d.ProductId == productId && d.Quantity > 0)
                .Include(d => d.Warehouse)
                .ThenInclude(w => w!.ProviderWarehouses!)
                .ThenInclude(pw => pw.Provider)
                .Include(d => d.Warehouse)
                .ThenInclude(w => w!.City);
        }
    }
}
