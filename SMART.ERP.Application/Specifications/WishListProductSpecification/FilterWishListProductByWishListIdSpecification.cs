using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.WishListProductSpecification
{
    public class FilterWishListProductByWishListIdSpecification : Specification<WishListProduct>
    {
        public FilterWishListProductByWishListIdSpecification(int id)
        {
            Query.Include(x => x.Product).Where(x => x.WishListId == id).AsNoTracking();
        }
    }
}
