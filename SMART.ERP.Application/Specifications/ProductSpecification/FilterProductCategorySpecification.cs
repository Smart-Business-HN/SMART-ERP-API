using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    public class FilterProductCategorySpecification : Specification<Product>
    {
        public FilterProductCategorySpecification(int categoryId)
        {
            Query.Include(x => x.Brand).Include(x => x.Status).Include(x => x.SubCategory)
                .Include(x => x.ProductImages).Include(x => x.ProductDataSheets!)
                .ThenInclude(x => x.DataSheet).Where(x => x.SubCategory!.CategoryId == categoryId).AsNoTracking();
        }
    }
}
