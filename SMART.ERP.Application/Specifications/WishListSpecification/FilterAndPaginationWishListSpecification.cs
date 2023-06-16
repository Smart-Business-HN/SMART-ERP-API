using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.WishListSpecification
{
    public class FilterAndPaginationWishListSpecification : Specification<WishList>
    {
        public FilterAndPaginationWishListSpecification(string? parameter, int pageNumber, int pageSize,
            string? order, string? column)
        {
            Query.Include(x => x.Status).Include(x => x.WishListProducts).ThenInclude(x => x.Product).ThenInclude(x => x!.Brand)
                .Include(x => x.WishListProducts).ThenInclude(x => x.Product).ThenInclude(x => x!.SubCategory).Skip((pageNumber) * pageSize).Take(pageSize)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Code.Contains(parameter) || x.Status!.Name.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "Code" ? x.Code : column == "Status" ? x.Status!.Name : null);
                }
                else
                {
                    Query.OrderBy(x => column == "Code" ? x.Code : column == "Status" ? x.Status!.Name : null);
                }
            }
        }
    }
}
