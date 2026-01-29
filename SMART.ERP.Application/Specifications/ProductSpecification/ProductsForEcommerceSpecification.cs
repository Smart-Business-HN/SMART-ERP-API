using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    public class ProductsForEcommerceSpecification : Specification<Product>
    {
        public ProductsForEcommerceSpecification()
        {
            Query.Where(x => x.ShowInEcommerce);
        }
    }
}
