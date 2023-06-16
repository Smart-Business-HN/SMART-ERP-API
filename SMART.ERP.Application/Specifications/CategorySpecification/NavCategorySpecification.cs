using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CategorySpecification
{
    public class NavCategorySpecification : Specification<Category>
    {
        public NavCategorySpecification()
        {
            Query.Include(x => x.Subcategories).AsNoTracking();
        }
    }
}
