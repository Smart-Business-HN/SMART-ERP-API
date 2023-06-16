using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProductImageFeature.Commands.DeleteProductImageCommand
{
    public class DeleteProductImageCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteProductImageCommandHandler : IRequestHandler<DeleteProductImageCommand, Response<string>>
    {
        private readonly IRepositoryAsync<ProductImage> _repositoryAsync;

        public DeleteProductImageCommandHandler(IRepositoryAsync<ProductImage> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
        {
            var productImage = await _repositoryAsync.GetByIdAsync(request.Id);
            if (productImage == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(productImage);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{productImage.FileName} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
