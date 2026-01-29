using MediatR;
using Microsoft.AspNetCore.Http;
using SMART.ERP.Application.DTOs.BlobStorage;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Services.BlobStorageService;
using SMART.ERP.Application.Wrappers;

namespace SMART.ERP.Application.Features.BlobStorageFeature.Commands.UploadBlobStorageCommand
{
    public class UploadBlobStorageCommand : IRequest<Response<StorageDto>>
    {
        public IFormFile File { get; set; } = null!;
    }

    public class UploadBlobStorageCommandHandler : IRequestHandler<UploadBlobStorageCommand, Response<StorageDto>>
    {
        private readonly IBlobStorageService _blobStorageService;
        public UploadBlobStorageCommandHandler(IBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }
        public async Task<Response<StorageDto>> Handle(UploadBlobStorageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _blobStorageService.UploadFileAsync(request.File);
                var getUrl = _blobStorageService.GetFile(request.File.FileName);
                var result = new StorageDto
                {
                    Url = getUrl,
                    FileName = request.File.FileName,
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
