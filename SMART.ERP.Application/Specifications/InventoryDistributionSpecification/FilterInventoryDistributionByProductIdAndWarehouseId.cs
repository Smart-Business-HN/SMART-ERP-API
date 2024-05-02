using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InventoryDistributionSpecification
{
    public class FilterInventoryDistributionByProductIdAndWarehouseId : Specification<InventoryDistribution>
    {
        public FilterInventoryDistributionByProductIdAndWarehouseId(int productId, int warehouseId)
        {
            Query.Include(x => x.Product).Where(x => x.ProductId == productId && x.WarehouseId == warehouseId);
        }
    }
}
