using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Services.BlobStorageService;
using SMART.ERP.Application.Wrappers;

namespace SMART.ERP.Application.Features.BlobStorageFeature.Commands.DeleteBlobStorageCommand
{
    public class DeleteBlobStorageCommand : IRequest<Response<string>>
    {
        public string FileName { get; set; } = null!;
    }

    public class DeleteBlobStorageCommandHandler : IRequestHandler<DeleteBlobStorageCommand, Response<string>>
    {
        private readonly IBlobStorageService _blobStorageService;

        public DeleteBlobStorageCommandHandler(IBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }
        public async Task<Response<string>> Handle(DeleteBlobStorageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Tolera tanto una URL completa (los assets guardan la URL absoluta) como un nombre/path
                // de blob con prefijo de carpeta. DeleteFileByUrlAsync conserva el prefijo y evita huérfanos.
                if (request.FileName.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    await _blobStorageService.DeleteFileByUrlAsync(request.FileName);
                }
                else
                {
                    await _blobStorageService.DeleteFileAsync(request.FileName);
                }
                return new Response<string>($"{request.FileName} eliminado correctamente", "Eliminado correctamente");
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message);
            }
        }
    }
}
