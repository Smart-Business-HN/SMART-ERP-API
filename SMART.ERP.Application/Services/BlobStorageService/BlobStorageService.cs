using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using SMART.ERP.Application.Exceptions;

namespace SMART.ERP.Application.Services.BlobStorageService
{
    public class BlobStorageService : IBlobStorageService
    {
        private const string ContainerName = "produccion";

        private readonly BlobServiceClient _blobServiceClient;
        public BlobStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(ContainerName);
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

        public async Task<bool> DeleteFileByUrlAsync(string blobUrl)
        {
            if (string.IsNullOrWhiteSpace(blobUrl))
            {
                return true;
            }

            // Deriva el nombre del blob (incl. cualquier prefijo de carpeta virtual) desde la URL completa,
            // independiente del nombre del contenedor. Path.GetFileName descartaría el prefijo de carpeta
            // y dejaría el blob anterior huérfano en storage.
            var blobName = new BlobUriBuilder(new Uri(blobUrl)).BlobName;
            return await DeleteFileAsync(blobName);
        }

        public string GetFile(string imageName)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(ContainerName);

            var blobClient = blobContainer.GetBlobClient(imageName);
            return blobClient.Uri.ToString();
        }

        public async Task UploadFileAsync(IFormFile model)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(ContainerName);

            var blobClient = blobContainer.GetBlobClient(model.FileName);

            var blobHttpHeader = new BlobHttpHeaders { ContentType = model.ContentType };

            try
            {
                await using var stream = model.OpenReadStream();
                await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeader });
            }
            catch
            {
                throw new ApiException("Ocurrio un error al cargar el archivo.");
            }
        }

        public async Task<string> UploadFileAndGetUrlAsync(IFormFile model, string? customFileName = null)
        {
            await using var stream = model.OpenReadStream();
            return await UploadFileAndGetUrlAsync(stream, customFileName ?? model.FileName, model.ContentType);
        }

        public async Task<string> UploadFileAndGetUrlAsync(Stream content, string fileName, string contentType)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(ContainerName);

            var blobClient = blobContainer.GetBlobClient(fileName);

            var blobHttpHeader = new BlobHttpHeaders { ContentType = contentType };

            try
            {
                if (content.CanSeek)
                {
                    content.Position = 0;
                }
                await blobClient.UploadAsync(content, new BlobUploadOptions { HttpHeaders = blobHttpHeader });
                return blobClient.Uri.ToString();
            }
            catch
            {
                throw new ApiException("Ocurrio un error al cargar el archivo.");
            }
        }
    }
}
