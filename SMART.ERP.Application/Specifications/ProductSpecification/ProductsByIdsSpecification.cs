using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    public class ProductsByIdsSpecification : Specification<Product>
    {
        public ProductsByIdsSpecification(IEnumerable<int> ids)
        {
            Query.Where(p => ids.Contains(p.Id)).AsNoTracking();
        }
    }
}
