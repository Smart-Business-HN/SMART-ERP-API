using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.WishListProductSpecification
{
    public class WishListProductIncludesSpecification : Specification<WishListProduct>
    {
        public WishListProductIncludesSpecification(int? id)
        {
            if (id != null)
            {
                Query.Include(x => x.Status).Include(x => x.Product)
                    .Include(x => x.Product).ThenInclude(x => x!.Brand).Where(x => x.Id == id).AsNoTracking();
            }

            else
            {
                Query.Include(x => x.Status).Include(x => x.Product)
                    .Include(x => x.Product).ThenInclude(x => x!.Brand).AsNoTracking();
            }
        }
    }
}
