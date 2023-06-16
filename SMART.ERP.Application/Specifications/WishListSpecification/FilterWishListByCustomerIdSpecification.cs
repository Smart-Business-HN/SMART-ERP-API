using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.WishListSpecification
{
	public class FilterWishListByCustomerIdSpecification : Specification<WishList>
	{
		public FilterWishListByCustomerIdSpecification(Guid customerId)
		{
			Query.Where(x => x.CustomerId == customerId && x.IsActive == true).AsNoTracking();
		}
	}
}

