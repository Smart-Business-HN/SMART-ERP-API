using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
namespace SMART.ERP.Application.Specifications.CartSpecification
{
    public sealed class FilterCartByCustomerIdSpecification : Specification<Cart>
    {
        public FilterCartByCustomerIdSpecification(Guid customerId, Guid? cartId)
        {
            if (cartId.HasValue)
            {
                Query.Where(cart => cart.Id == cartId.Value && cart.EcommerceUserId == customerId && cart.IsActive);
            }
            else
            {
                Query.Where(cart => cart.EcommerceUserId == customerId && cart.IsActive);
            }
            Query.Include(cart => cart.CartItems);
        }
    }
}
