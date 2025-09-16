using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Cart;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CartSpecification;
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
    }
    public class AddProductToCartCommandHandler : IRequestHandler<AddProductToCartCommand, Response<CartDto>>
    {
        private IMapper _mapper;
        private readonly IRepositoryAsync<Cart> _cartRepositoryAsync;
        private readonly IRepositoryAsync<CartItem> _cartItemRepositoryAsync;
       public AddProductToCartCommandHandler(IMapper mapper, IRepositoryAsync<Cart> cartRepositoryAsync, IRepositoryAsync<CartItem> cartItemRepositoryAsync)
        {
            _mapper = mapper;
            _cartRepositoryAsync = cartRepositoryAsync;
            _cartItemRepositoryAsync = cartItemRepositoryAsync;
        }
        public async Task<Response<CartDto>> Handle(AddProductToCartCommand request, CancellationToken cancellationToken)
        {
            var cartActive = await _cartRepositoryAsync.FirstOrDefaultAsync(new FilterCartByCustomerIdSpecification(request.CustomerId,request.CartId));
            if (cartActive != null && cartActive.EcommerceUserId != request.CustomerId)
            {
                throw new KeyNotFoundException($"The cart with id {request.CartId} does not belong to the customer with id {request.CustomerId}.");
            }
            if (cartActive == null)
            {
                var newCart = new Cart
                {
                    EcommerceUserId = request.CustomerId,
                    CreationDate = DateTime.UtcNow,
                    IsActive = true
                };
                await _cartRepositoryAsync.AddAsync(newCart);
                cartActive = newCart;
            }
            var cartItem = new CartItem
            {
                CartId = cartActive.Id,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                Discount = 0,
                CreationDate = DateTime.UtcNow
            };
            var addedCartItem = await _cartItemRepositoryAsync.AddAsync(cartItem);
            cartActive.CartItems ??= new List<CartItem>();
            cartActive.CartItems.Add(addedCartItem);
            var dto = _mapper.Map<CartDto>(cartActive);
            return new Response<CartDto>(dto);
        }
    }
}


