using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.BlobStorageService;
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
        private readonly IBlobStorageService _blobStorageService;

        public DeleteProductImageCommandHandler(
            IRepositoryAsync<ProductImage> repositoryAsync,
            IBlobStorageService blobStorageService)
        {
            _repositoryAsync = repositoryAsync;
            _blobStorageService = blobStorageService;
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

            // Borrado best-effort del blob para no dejar archivos huérfanos en storage.
            try { await _blobStorageService.DeleteFileByUrlAsync(productImage.Url); } catch { /* no bloquear el delete */ }

            return new Response<string>($"{productImage.FileName} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
