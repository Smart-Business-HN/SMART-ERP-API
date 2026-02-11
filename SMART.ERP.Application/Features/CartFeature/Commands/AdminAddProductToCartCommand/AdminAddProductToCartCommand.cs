using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Cart;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services;
using SMART.ERP.Application.Specifications.CartSpecification;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CartFeature.Commands.AdminAddProductToCartCommand;

public class AdminAddProductToCartCommand : IRequest<Response<CartDto>>
{
    public Guid EcommerceUserId { get; set; }
    public Guid? CartId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public bool ForceNewCart { get; set; }
    public string? ProductDescription { get; set; }
}

public class AdminAddProductToCartCommandHandler : IRequestHandler<AdminAddProductToCartCommand, Response<CartDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryAsync<Cart> _cartRepositoryAsync;
    private readonly IRepositoryAsync<CartItem> _cartItemRepositoryAsync;
    private readonly IRepositoryAsync<Product> _productRepositoryAsync;
    private readonly IRepositoryAsync<EcommerceUser> _ecommerceUserRepositoryAsync;
    private readonly IProductPricingService _productPricingService;

    public AdminAddProductToCartCommandHandler(
        IMapper mapper,
        IRepositoryAsync<Cart> cartRepositoryAsync,
        IRepositoryAsync<CartItem> cartItemRepositoryAsync,
        IRepositoryAsync<Product> productRepositoryAsync,
        IProductPricingService productPricingService,
        IRepositoryAsync<EcommerceUser> ecommerceUserRepositoryAsync)
    {
        _mapper = mapper;
        _cartRepositoryAsync = cartRepositoryAsync;
        _cartItemRepositoryAsync = cartItemRepositoryAsync;
        _productRepositoryAsync = productRepositoryAsync;
        _productPricingService = productPricingService;
        _ecommerceUserRepositoryAsync = ecommerceUserRepositoryAsync;
    }

    public async Task<Response<CartDto>> Handle(AdminAddProductToCartCommand request, CancellationToken cancellationToken)
    {
        var user = await _ecommerceUserRepositoryAsync.GetByIdAsync(request.EcommerceUserId, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException($"Usuario ecommerce no encontrado con el id: {request.EcommerceUserId}");

        var product = await _productRepositoryAsync.FirstOrDefaultAsync(
            new FilterProductSpecification(null, request.ProductId, null), cancellationToken);
        if (product == null)
            throw new KeyNotFoundException($"Producto no encontrado con el id: {request.ProductId}");

        Cart? cart = null;

        if (request.ForceNewCart)
        {
            // Force create a new cart regardless of existing ones
            cart = new Cart
            {
                EcommerceUserId = request.EcommerceUserId,
                CreationDate = DateTime.UtcNow,
                IsActive = true
            };
            await _cartRepositoryAsync.AddAsync(cart, cancellationToken);
        }
        else if (request.CartId.HasValue)
        {
            cart = await _cartRepositoryAsync.FirstOrDefaultAsync(
                new FilterCartByCustomerIdSpecification(request.EcommerceUserId, request.CartId), cancellationToken);
            if (cart == null)
                throw new KeyNotFoundException($"Carrito no encontrado con el id: {request.CartId}");

            // Check for duplicate product in this cart
            if (cart.CartItems != null && cart.CartItems.Any(ci => ci.ProductId == request.ProductId))
                throw new ApplicationException($"Este producto ya existe en el carrito.");
        }
        else
        {
            cart = await _cartRepositoryAsync.FirstOrDefaultAsync(
                new FilterCartByCustomerIdSpecification(request.EcommerceUserId, null), cancellationToken);

            if (cart == null)
            {
                cart = new Cart
                {
                    EcommerceUserId = request.EcommerceUserId,
                    CreationDate = DateTime.UtcNow,
                    IsActive = true
                };
                await _cartRepositoryAsync.AddAsync(cart, cancellationToken);
            }
            else
            {
                // Check for duplicate product in existing cart
                if (cart.CartItems != null && cart.CartItems.Any(ci => ci.ProductId == request.ProductId))
                    throw new ApplicationException($"Este producto ya existe en el carrito.");
            }
        }

        var unitPrice = request.UnitPrice ?? _productPricingService.CalculateRecommendedSalePrice(product, true, user.CustomerTypeId);

        var cartItem = new CartItem
        {
            CartId = cart.Id,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            Discount = 0,
            UnitPrice = unitPrice,
            TotalPrice = request.Quantity * unitPrice,
            CreationDate = DateTime.UtcNow,
            ProductDescription = request.ProductDescription ?? product.Name
        };

        await _cartItemRepositoryAsync.AddAsync(cartItem, cancellationToken);

        var reloadedCart = await _cartRepositoryAsync.FirstOrDefaultAsync(
            new GetCartByIdSpecification(cart.Id), cancellationToken);
        var dto = _mapper.Map<CartDto>(reloadedCart);
        return new Response<CartDto>(dto, "Producto agregado al carrito exitosamente.");
    }
}
