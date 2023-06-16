using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductImageSpecification
{
    public class FilterProductImageFromFilenameSpecification : Specification<ProductImage>
    {
        public FilterProductImageFromFilenameSpecification(string fileName)
        {
            Query.Where(a => a.FileName == fileName).AsNoTracking();
        }
    }
}
