using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    public class FilterAndPaginationProductForEcommerceSpecification : Specification<Product>
    {
        public FilterAndPaginationProductForEcommerceSpecification(string? parameter, int pageNumber, int pageSize,
            string? order, string? column)
        {
            Query.Include(x => x.SubCategory)
                .Include(x => x.Status)
                .Include(x => x.Brand)
                .Include(x => x.ProductImages)
                .Include(x => x.Tax)
                .Skip((pageNumber) * pageSize).Take(pageSize).Where(x => x.ShowInEcommerce).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Name.Contains(parameter) && x.ShowInEcommerce || x.Code.Contains(parameter) && x.ShowInEcommerce
                || x.SubCategory!.Name.Contains(parameter) && x.ShowInEcommerce || x.Brand!.Name.Contains(parameter) && x.ShowInEcommerce);
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "Name" ? x.Name
                    : column == "Code" ? x.Code : column == "SubCategory" ? x.SubCategory!.Name
                    : column == "Brand" ? x.Brand!.Name : null).Where(x => x.ShowInEcommerce);
                }
                else
                {
                    Query.OrderBy(x => column == "Name" ? x.Name
                    : column == "Code" ? x.Code : column == "SubCategory" ? x.SubCategory!.Name
                    : column == "Brand" ? x.Brand!.Name : null).Where(x => x.ShowInEcommerce);
                }
            }
        }
    }
}
