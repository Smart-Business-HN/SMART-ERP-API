using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InventoryDistributionSpecification
{
    public class FilterInventoryByProductSpec : Specification<InventoryDistribution>
    {
        public FilterInventoryByProductSpec(int productId)
        {
            Query.Where(d => d.ProductId == productId);
        }
    }
}
