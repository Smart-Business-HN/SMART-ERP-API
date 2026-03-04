using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Cart;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CartSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CartFeature.Commands.AdminUpdateCartItemCommand;

public class AdminUpdateCartItemCommand : IRequest<Response<CartDto>>
{
    public int CartItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? Discount { get; set; }
    public string? ProductDescription { get; set; }
}

public class AdminUpdateCartItemCommandHandler : IRequestHandler<AdminUpdateCartItemCommand, Response<CartDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryAsync<CartItem> _cartItemRepositoryAsync;
    private readonly IRepositoryAsync<Cart> _cartRepositoryAsync;

    public AdminUpdateCartItemCommandHandler(
        IMapper mapper,
        IRepositoryAsync<CartItem> cartItemRepositoryAsync,
        IRepositoryAsync<Cart> cartRepositoryAsync)
    {
        _mapper = mapper;
        _cartItemRepositoryAsync = cartItemRepositoryAsync;
        _cartRepositoryAsync = cartRepositoryAsync;
    }

    public async Task<Response<CartDto>> Handle(AdminUpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        var cartItem = await _cartItemRepositoryAsync.GetByIdAsync(request.CartItemId, cancellationToken);
        if (cartItem == null)
            throw new KeyNotFoundException($"Item del carrito no encontrado con el id: {request.CartItemId}");

        var cart = await _cartRepositoryAsync.FirstOrDefaultAsync(
            new GetCartByIdSpecification(cartItem.CartId), cancellationToken);
        if (cart != null && cart.Status != Domain.Enums.CartStatus.Active)
            throw new ApplicationException("No se pueden modificar productos de un carrito en proceso de pago.");

        cartItem.Quantity = request.Quantity;
        cartItem.UnitPrice = request.UnitPrice;
        cartItem.Discount = request.Discount ?? 0;
        cartItem.ProductDescription = request.ProductDescription;

        await _cartItemRepositoryAsync.UpdateAsync(cartItem, cancellationToken);

        var reloadedCart = await _cartRepositoryAsync.FirstOrDefaultAsync(
            new GetCartByIdSpecification(cartItem.CartId), cancellationToken);
        var dto = _mapper.Map<CartDto>(reloadedCart);
        return new Response<CartDto>(dto, "Item del carrito actualizado exitosamente.");
    }
}
