using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductFeatureSpecification
{
    public class ProductFeatureByProjectSpecification : Specification<ProductFeature>
    {
        public ProductFeatureByProjectSpecification(int productId)
        {
            Query.Where(x => x.ProductId == productId);
        }
    }
}
