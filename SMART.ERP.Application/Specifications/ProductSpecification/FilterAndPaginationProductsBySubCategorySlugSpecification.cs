using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    public class FilterAndPaginationProductsBySubCategorySlugSpecification : Specification<Product>
    {
        public FilterAndPaginationProductsBySubCategorySlugSpecification(string subCategorySlug, string? parameter, int pageNumber, int pageSize,
            string? order, string? column)
        {
            Query.Include(x => x.SubCategory).ThenInclude(x => x.Category).Include(x => x.Status).Include(x => x.Brand).Include(x => x.Tax)
                .Include(x => x.ProductImages).Include(x => x.ProductDataSheets!).ThenInclude(x => x.DataSheet)
            .Skip((pageNumber) * pageSize).Take(pageSize).Where(x => x.ShowInEcommerce && x.SubCategory.Slug == subCategorySlug).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Name.Contains(parameter) && x.ShowInEcommerce && x.SubCategory.Slug == subCategorySlug || x.Code.Contains(parameter) && x.ShowInEcommerce && x.SubCategory.Slug == subCategorySlug
                || x.SubCategory!.Name.Contains(parameter) && x.ShowInEcommerce && x.SubCategory.Slug == subCategorySlug || x.Description.Contains(parameter) && x.ShowInEcommerce && x.SubCategory.Slug == subCategorySlug || x.Brand!.Name.Contains(parameter) && x.ShowInEcommerce && x.SubCategory.Slug == subCategorySlug);
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "Name" ? x.Name
                    : column == "Code" ? x.Code : column == "SubCategory" ? x.SubCategory!.Name
                    : column == "Brand" ? x.Brand!.Name : null).Where(x => x.ShowInEcommerce && x.SubCategory.Slug == subCategorySlug);
                }
                else
                {
                    Query.OrderBy(x => column == "Name" ? x.Name
                    : column == "Code" ? x.Code : column == "SubCategory" ? x.SubCategory!.Name
                    : column == "Brand" ? x.Brand!.Name : null).Where(x => x.ShowInEcommerce && x.SubCategory.Slug == subCategorySlug);
                }
            }
        }
    }
}
