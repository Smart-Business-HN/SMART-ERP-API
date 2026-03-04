using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Cart;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services;
using SMART.ERP.Application.Specifications.CartSpecification;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CartFeature.Commands.AddProductToCartCommand
{
    public class AddProductToCartCommand : IRequest<Response<CartDto>>
    {
       public Guid CustomerId { get; set; }
        public Guid? CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public bool ForceNewCart { get; set; }
    }
    public class AddProductToCartCommandHandler : IRequestHandler<AddProductToCartCommand, Response<CartDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Cart> _cartRepositoryAsync;
        private readonly IRepositoryAsync<CartItem> _cartItemRepositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        private readonly IRepositoryAsync<EcommerceUser> _ecommerceUserRepositoryAsync;
        private readonly IProductPricingService _productPricingService;
       public AddProductToCartCommandHandler(IMapper mapper, IRepositoryAsync<Cart> cartRepositoryAsync, IRepositoryAsync<CartItem> cartItemRepositoryAsync, IRepositoryAsync<Product> productRepositoryAsync, IProductPricingService productPricingService, IRepositoryAsync<EcommerceUser> ecommerceUserRepositoryAsync)
        {
            _mapper = mapper;
            _cartRepositoryAsync = cartRepositoryAsync;
            _cartItemRepositoryAsync = cartItemRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
            _productPricingService = productPricingService;
            _ecommerceUserRepositoryAsync = ecommerceUserRepositoryAsync;
        }
        public async Task<Response<CartDto>> Handle(AddProductToCartCommand request, CancellationToken cancellationToken)
        {
            var user = await _ecommerceUserRepositoryAsync.GetByIdAsync(request.CustomerId, cancellationToken);
            if (user == null)
            {
                throw new ApplicationException($"Couldn't find user with id: {request.CustomerId}");
            }
            var product = await _productRepositoryAsync.FirstOrDefaultAsync(new FilterProductSpecification(null,request.ProductId,null), cancellationToken);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product not found with id {request.ProductId}");
            }
            Cart cartActive;
            if (request.ForceNewCart)
            {
                cartActive = new Cart
                {
                    EcommerceUserId = request.CustomerId,
                    CreationDate = DateTime.UtcNow,
                    IsActive = true
                };
                await _cartRepositoryAsync.AddAsync(cartActive, cancellationToken);
            }
            else
            {
                cartActive = await _cartRepositoryAsync.FirstOrDefaultAsync(new FilterCartByCustomerIdSpecification(request.CustomerId,request.CartId), cancellationToken);
                if (cartActive != null && cartActive.EcommerceUserId != request.CustomerId)
                {
                    throw new KeyNotFoundException($"The cart with id {request.CartId} does not belong to the customer with id {request.CustomerId}.");
                }
                if (cartActive != null && cartActive.Status != Domain.Enums.CartStatus.Active)
                {
                    throw new ApplicationException("No se pueden modificar productos de un carrito en proceso de pago.");
                }
                if (cartActive == null)
                {
                    cartActive = new Cart
                    {
                        EcommerceUserId = request.CustomerId,
                        CreationDate = DateTime.UtcNow,
                        IsActive = true
                    };
                    await _cartRepositoryAsync.AddAsync(cartActive, cancellationToken);
                }
            }
            var cartItem = new CartItem
            {
                CartId = cartActive.Id,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                Discount = 0,
                UnitPrice = _productPricingService.CalculateRecommendedSalePrice(product,true,user.CustomerTypeId),
                TotalPrice = (request.Quantity * _productPricingService.CalculateRecommendedSalePrice(product,true,user.CustomerTypeId)),
                CreationDate = DateTime.UtcNow
            };
            var addedCartItem = await _cartItemRepositoryAsync.AddAsync(cartItem, cancellationToken);
            cartActive.CartItems ??= new List<CartItem>();
            cartActive.CartItems.Add(addedCartItem);
            var dto = _mapper.Map<CartDto>(cartActive);
            return new Response<CartDto>(dto);
        }
    }
}


