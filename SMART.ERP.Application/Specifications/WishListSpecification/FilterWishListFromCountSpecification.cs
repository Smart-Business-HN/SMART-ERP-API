using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.WishListSpecification
{
    public class FilterWishListFromCountSpecification : Specification<WishList>
    {
        public FilterWishListFromCountSpecification(Guid customerId, DateTime date)
        {
            Query.Where(x => x.CustomerId == customerId && x.CreationDate.Date == date.Date).AsNoTracking();
        }
    }
}
