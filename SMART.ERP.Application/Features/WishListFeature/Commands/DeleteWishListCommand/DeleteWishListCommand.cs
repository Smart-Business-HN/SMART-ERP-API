using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.WishListSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.WishListFeature.Commands.DeleteWishListCommand
{
    public class DeleteWishListCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteWishListCommandHandler : IRequestHandler<DeleteWishListCommand, Response<string>>
    {
        private readonly IRepositoryAsync<WishList> _repositoryAsync;
        private readonly IRepositoryAsync<WishListProduct> _wishListProductRepositoryAsync;

        public DeleteWishListCommandHandler(IRepositoryAsync<WishList> repositoryAsync,
            IRepositoryAsync<WishListProduct> wishListProductRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _wishListProductRepositoryAsync = wishListProductRepositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteWishListCommand request, CancellationToken cancellationToken)
        {
            var wishList = await _repositoryAsync.FirstOrDefaultAsync(
                new WishListIncludesSpecification(id: request.Id, code: null, customerId: null));
            if (wishList == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            foreach (var item in wishList.WishListProducts)
            {
                await _wishListProductRepositoryAsync.DeleteAsync(item);
                await _wishListProductRepositoryAsync.SaveChangesAsync();
            }
            await _repositoryAsync.DeleteAsync(wishList);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{wishList.Code} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
