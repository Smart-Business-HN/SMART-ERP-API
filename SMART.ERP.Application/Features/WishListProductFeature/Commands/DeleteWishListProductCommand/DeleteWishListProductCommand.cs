using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.WishListProductFeature.Commands.DeleteWishListProductCommand
{
    public class DeleteWishListProductCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }
    public class DeleteWishListProductCommandHandler : IRequestHandler<DeleteWishListProductCommand, Response<string>>
    {
        private readonly IRepositoryAsync<WishListProduct> _repositoryAsync;

        public DeleteWishListProductCommandHandler(IRepositoryAsync<WishListProduct> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteWishListProductCommand request, CancellationToken cancellationToken)
        {
            var wishListProduct = await _repositoryAsync.GetByIdAsync(request.Id);
            if (wishListProduct == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(wishListProduct);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"Producto eliminado correctamente", "Eliminado correctamente");
        }
    }
}
