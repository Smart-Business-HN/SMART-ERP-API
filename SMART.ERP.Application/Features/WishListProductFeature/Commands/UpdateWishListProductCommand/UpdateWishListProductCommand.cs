using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.WishList;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.WishListProductFeature.Commands.UpdateWishListProductCommand
{
    public class UpdateWishListProductCommand : IRequest<Response<WishListProductDto>>
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
    }
    public class UpdateWishListProductCommandHandler : IRequestHandler<UpdateWishListProductCommand, Response<WishListProductDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<WishListProduct> _repositoryAsync;
        private readonly IRepositoryAsync<WishList> _wishListRepositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        public UpdateWishListProductCommandHandler(IMapper mapper, IRepositoryAsync<WishListProduct> repositoryAsync, IRepositoryAsync<WishList> wishListRepositoryAsync, IRepositoryAsync<Product> productRepositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _wishListRepositoryAsync = wishListRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
        }
        public async Task<Response<WishListProductDto>> Handle(UpdateWishListProductCommand request, CancellationToken cancellationToken)
        {
            var wishListProduct = await _repositoryAsync.GetByIdAsync(request.Id);
            if (wishListProduct == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var product = await _productRepositoryAsync.GetByIdAsync(wishListProduct!.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun producto con el id {wishListProduct!.ProductId}");
            }
            var wishList = await _wishListRepositoryAsync.GetByIdAsync(wishListProduct!.WishListId);
            if (wishList == null)
            {
                throw new KeyNotFoundException($"No se encontro ninguna lista de deseo con el id {wishListProduct!.ProductId}");
            }
            if (wishListProduct!.Quantity < request.Quantity)
            {
                wishList.CantItems += request.Quantity - wishListProduct!.Quantity;
            }
            else
            {
                wishList.CantItems -= wishListProduct!.Quantity - request.Quantity;
            }
            await _wishListRepositoryAsync.UpdateAsync(wishList);
            await _wishListRepositoryAsync.SaveChangesAsync();
            wishListProduct!.Quantity = request.Quantity;
            wishListProduct!.Product = product;
            await _repositoryAsync.UpdateAsync(wishListProduct);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<WishListProductDto>(wishListProduct);
            return new Response<WishListProductDto>(dto, message: $"Item actualizado correctamente");
        }
    }
}

