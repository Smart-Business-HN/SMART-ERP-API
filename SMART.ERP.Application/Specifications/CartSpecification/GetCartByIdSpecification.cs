using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CartSpecification;

public class GetCartByIdSpecification : Specification<Cart>
{
    public GetCartByIdSpecification(Guid cartId)
    {
        Query.Where(cart => cart.Id == cartId);
        Query.Include(x=>x.EcommerceUser).ThenInclude(eu=>eu.CustomerType);
        Query.Include(cart => cart.CartItems).ThenInclude(ci => ci.Product).ThenInclude(p=>p.ProductImages);
        Query.Include(cart => cart.CartItems).ThenInclude(ci => ci.Product).ThenInclude(p=>p.Brand);
    }
}