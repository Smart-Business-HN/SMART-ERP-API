using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Cart;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CartSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CartFeature.Commands.AdminRemoveCartItemCommand;

public class AdminRemoveCartItemCommand : IRequest<Response<CartDto>>
{
    public int CartItemId { get; set; }
}

public class AdminRemoveCartItemCommandHandler : IRequestHandler<AdminRemoveCartItemCommand, Response<CartDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryAsync<CartItem> _cartItemRepositoryAsync;
    private readonly IRepositoryAsync<Cart> _cartRepositoryAsync;

    public AdminRemoveCartItemCommandHandler(
        IMapper mapper,
        IRepositoryAsync<CartItem> cartItemRepositoryAsync,
        IRepositoryAsync<Cart> cartRepositoryAsync)
    {
        _mapper = mapper;
        _cartItemRepositoryAsync = cartItemRepositoryAsync;
        _cartRepositoryAsync = cartRepositoryAsync;
    }

    public async Task<Response<CartDto>> Handle(AdminRemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        var cartItem = await _cartItemRepositoryAsync.GetByIdAsync(request.CartItemId, cancellationToken);
        if (cartItem == null)
            throw new KeyNotFoundException($"Item del carrito no encontrado con el id: {request.CartItemId}");

        var cartId = cartItem.CartId;

        var cart = await _cartRepositoryAsync.FirstOrDefaultAsync(
            new GetCartByIdSpecification(cartId), cancellationToken);
        if (cart != null && cart.Status != Domain.Enums.CartStatus.Active)
            throw new ApplicationException("No se pueden modificar productos de un carrito en proceso de pago.");

        await _cartItemRepositoryAsync.DeleteAsync(cartItem, cancellationToken);

        var reloadedCart = await _cartRepositoryAsync.FirstOrDefaultAsync(
            new GetCartByIdSpecification(cartId), cancellationToken);
        var dto = _mapper.Map<CartDto>(reloadedCart);
        return new Response<CartDto>(dto, "Producto eliminado del carrito exitosamente.");
    }
}
