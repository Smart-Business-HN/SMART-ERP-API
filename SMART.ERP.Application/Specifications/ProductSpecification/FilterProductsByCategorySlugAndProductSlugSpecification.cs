using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    public sealed class FilterProductsByCategorySlugAndProductSlugSpecification : Specification<Product>
    {
        public FilterProductsByCategorySlugAndProductSlugSpecification(string categorySlug, string productSlug) 
        {
            Query.Include(x => x.Brand)
                .Include(x => x.ProductImages)
                .Include(x => x.SubCategory).ThenInclude(x => x!.Category)
                .Include(x => x.Status)
                .Where(x => x.ProductSubcategories!.Any(ps => ps.Subcategory!.Category!.Slug == categorySlug) && x.Slug != productSlug && x.ShowInEcommerce == true).AsNoTracking();
        }
    }
}
