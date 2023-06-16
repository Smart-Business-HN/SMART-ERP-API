using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CategorySpecification
{
    public class IncludeCategorySpecification : Specification<Category>
    {
        public IncludeCategorySpecification()
        {
            Query.Include(x => x.Subcategories).AsNoTracking();
        }
    }
}
