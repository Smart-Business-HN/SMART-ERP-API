using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using SMART.ERP.Application.Exceptions;

namespace SMART.ERP.Application.Services.BlobStorageService
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        public BlobStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient("produccion");
            var blobClient = blobContainer.GetBlobClient(fileName);
            if (await blobClient.ExistsAsync())
            {
                try
                {
                    await blobClient.DeleteAsync();
                }
                catch
                {
                    throw new ApiException("Ocurrio un error al eliminar el archivo.");
                }
            }
            return true;
        }

        public string GetFile(string imageName)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient("produccion");

            var blobClient = blobContainer.GetBlobClient(imageName);
            return blobClient.Uri.ToString();
        }

        public async Task UploadFileAsync(IFormFile model)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient("produccion");

            var blobClient = blobContainer.GetBlobClient(model.FileName);

            var blobHttpHeader = new BlobHttpHeaders { ContentType = model.ContentType };

            try
            {
                await blobClient.UploadAsync(model.OpenReadStream(), new BlobUploadOptions { HttpHeaders = blobHttpHeader });
            }
            catch
            {
                throw new ApiException("Ocurrio un error al cargar el archivo.");
            }
        }
    }
}
