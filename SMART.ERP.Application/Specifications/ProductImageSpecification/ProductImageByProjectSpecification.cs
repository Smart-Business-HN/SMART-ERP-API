using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductImageSpecification
{
    public class ProductImageByProjectSpecification : Specification<ProductImage>
    {
        public ProductImageByProjectSpecification(int productId)
        {
            Query.Where(x => x.ProductId == productId);
        }
    }
}
