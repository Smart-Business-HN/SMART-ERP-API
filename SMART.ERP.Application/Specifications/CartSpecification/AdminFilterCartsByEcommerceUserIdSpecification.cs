using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CartSpecification;

public sealed class AdminFilterCartsByEcommerceUserIdSpecification : Specification<Cart>
{
    public AdminFilterCartsByEcommerceUserIdSpecification(Guid ecommerceUserId, bool? isActive = null)
    {
        if (isActive.HasValue)
        {
            Query.Where(cart => cart.EcommerceUserId == ecommerceUserId && cart.IsActive == isActive.Value);
        }
        else
        {
            Query.Where(cart => cart.EcommerceUserId == ecommerceUserId);
        }

        Query.Include(cart => cart.CartItems).ThenInclude(ci => ci.Product).ThenInclude(p => p.Brand);
        Query.Include(cart => cart.CartItems).ThenInclude(ci => ci.Product).ThenInclude(p => p.ProductImages);
        Query.OrderByDescending(cart => cart.CreationDate);
    }
}
