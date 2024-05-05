using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CategorySpecification
{
    public class GetCategoryBySlugSpecification : Specification<Category>
    {
        public GetCategoryBySlugSpecification(string slug)
        {
            Query.Where(x => x.Slug == slug).AsNoTracking();
        }
    }
}
