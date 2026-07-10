using MediatR;
using Microsoft.AspNetCore.Http;
using SMART.ERP.Application.DTOs.BlobStorage;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Services.BlobStorageService;
using SMART.ERP.Application.Services.ImageOptimizationService;
using SMART.ERP.Application.Wrappers;

namespace SMART.ERP.Application.Features.BlobStorageFeature.Commands.UploadBlobStorageCommand
{
    public class UploadBlobStorageCommand : IRequest<Response<StorageDto>>
    {
        public IFormFile File { get; set; } = null!;

        /// <summary>
        /// Carpeta destino dentro del contenedor (products, categories, brands, banners, avatars, ...).
        /// Si es nula o inválida, el archivo cae en la carpeta "misc". Ver <see cref="BlobFolders"/>.
        /// </summary>
        public string? Folder { get; set; }
    }

    public class UploadBlobStorageCommandHandler : IRequestHandler<UploadBlobStorageCommand, Response<StorageDto>>
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly IImageOptimizationService _imageOptimizationService;

        public UploadBlobStorageCommandHandler(
            IBlobStorageService blobStorageService,
            IImageOptimizationService imageOptimizationService)
        {
            _blobStorageService = blobStorageService;
            _imageOptimizationService = imageOptimizationService;
        }

        public async Task<Response<StorageDto>> Handle(UploadBlobStorageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var folder = BlobFolders.NormalizeOrDefault(request.Folder);

                // Optimiza/convierte a WebP (pass-through seguro si no es imagen o el optimizado no es más chico).
                var optimization = await _imageOptimizationService.OptimizeAsync(request.File, cancellationToken);
                var extension = optimization.WasOptimized
                    ? optimization.Extension!
                    : Path.GetExtension(request.File.FileName);

                // Nombre único bajo la carpeta del asset: products/<guid>.webp
                var blobName = $"{folder}/{Guid.NewGuid():N}{extension}";

                var url = await _blobStorageService.UploadOptimizedOrOriginalAsync(request.File, optimization, blobName);

                var result = new StorageDto
                {
                    Url = url,
                    FileName = blobName,
                };
                return new Response<StorageDto>(result, message: $"{request.File.FileName} subido exitosamente");
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message);
            }
        }
    }
}
