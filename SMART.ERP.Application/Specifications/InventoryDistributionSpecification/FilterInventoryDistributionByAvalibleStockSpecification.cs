using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InventoryDistributionSpecification
{
    public class FilterInventoryDistributionByAvalibleStockSpecification : Specification<InventoryDistribution>
    {
        public FilterInventoryDistributionByAvalibleStockSpecification()
        {
            Query.Include(x => x.Product).ThenInclude(x => x.Tax).Where(x => x.Quantity > 0);
        }
    }
}
