using Microsoft.AspNetCore.Http;

namespace SMART.ERP.Application.Services.BlobStorageService
{
    public interface IBlobStorageService
    {
        Task UploadFileAsync(IFormFile model);

        Task<string> UploadFileAndGetUrlAsync(IFormFile model, string? customFileName = null);

        Task<string> UploadFileAndGetUrlAsync(Stream content, string fileName, string contentType);

        string GetFile(string imageName);

        Task<bool> DeleteFileAsync(string fileName);

        Task<bool> DeleteFileByUrlAsync(string blobUrl);
    }
}
