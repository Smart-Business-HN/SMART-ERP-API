using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    public class FilterProductSubcategorySpecification : Specification<Product>
    {
        public FilterProductSubcategorySpecification(int subcategoryId)
        {
            Query.Where(x => x.ProductSubcategories!.Any(ps => ps.SubcategoryId == subcategoryId)).Include(x => x.Brand).Include(x => x.Status).Include(x => x.ProductImages)
                .Include(x => x.ProductDataSheets!).ThenInclude(x => x.DataSheet).AsNoTracking();
        }
    }
}
