using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    public class FilterNavSpecification : Specification<Product>
    {
        public FilterNavSpecification(int subCategoryId)
        {
            Query.Include(x => x.SubCategory).Where(x => x.ProductSubcategories!.Any(ps => ps.SubcategoryId == subCategoryId)).AsNoTracking();
        }
    }
}
