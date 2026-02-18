using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    public class FilterProductByCodeSpec : Specification<Product>
    {
        public FilterProductByCodeSpec(string code)
        {
            Query.Where(p => p.Code == code);
        }
    }
}
