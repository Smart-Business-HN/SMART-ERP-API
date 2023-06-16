using Microsoft.AspNetCore.Http;

namespace SMART.ERP.Application.Services.BlobStorageService
{
    public interface IBlobStorageService
    {
        Task UploadFileAsync(IFormFile model);

        string GetFile(string imageName);

        Task<bool> DeleteFileAsync(string fileName);
    }
}
