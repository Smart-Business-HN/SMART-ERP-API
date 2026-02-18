using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InventoryDistributionSpecification
{
    public class FilterInventoryByProductAndWarehouseSpec : Specification<InventoryDistribution>
    {
        public FilterInventoryByProductAndWarehouseSpec(int productId, int warehouseId)
        {
            Query.Where(d => d.ProductId == productId && d.WarehouseId == warehouseId);
        }
    }
}
