using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.WishListSpecification
{
    public class FilterLastWishListSpecification : Specification<WishList>
    {
        public FilterLastWishListSpecification()
        {
            Query.OrderByDescending(x => x.Id).Take(1).AsNoTracking();
        }
    }
}
