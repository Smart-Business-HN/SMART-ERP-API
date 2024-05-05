using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.SubcategorySpecification
{
    public class GetSubCategoryBySlugSpecification : Specification<Subcategory>
    {
        public GetSubCategoryBySlugSpecification(string subCategorySlug)
        {
            Query.Where(x => x.Slug == subCategorySlug).AsNoTracking();
        }
    }
}
