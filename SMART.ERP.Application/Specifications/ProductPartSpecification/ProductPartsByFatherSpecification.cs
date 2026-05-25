using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductPartSpecification
{
    public class ProductPartsByFatherSpecification : Specification<ProductPart>
    {
        public ProductPartsByFatherSpecification(int fatherProductId, bool? onlyActive = null)
        {
            Query.Where(p => p.FatherProductId == fatherProductId);
            if (onlyActive == true)
                Query.Where(p => p.IsActive);
        }
    }
}
