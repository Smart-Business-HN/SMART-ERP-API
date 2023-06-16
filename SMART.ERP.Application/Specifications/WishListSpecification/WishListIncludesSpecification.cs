using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.WishListSpecification
{
    public class WishListIncludesSpecification : Specification<WishList>
    {
        public WishListIncludesSpecification(int? id, string? code, Guid? customerId)
        {
            if (id != null)
            {
                Query.Include(x => x.Status).Include(x => x.WishListProducts).ThenInclude(x => x.Product)
                    .ThenInclude(x => x!.SubCategory).Include(x => x.WishListProducts).ThenInclude(x => x.Product).ThenInclude(x => x!.ProductImages).Where(x => x.Id == id);
            }
            else if (code != null)
            {
                Query.Include(x => x.Status).Include(x => x.WishListProducts).Where(x => x.Code == code);
            }
            else if (customerId != null)
            {
                Query.Include(x => x.Status).Include(x => x.WishListProducts).OrderByDescending(x => x.Id)
                    .Where(x => x.CustomerId == customerId && x.IsActive);
            }
            else
            {
                Query.Include(x => x.Status).Include(x => x.WishListProducts);
            }
        }
    }
}
