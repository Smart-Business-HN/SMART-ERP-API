using Microsoft.AspNetCore.Http;
using SMART.ERP.Application.Services.BlobStorageService;

namespace SMART.ERP.Application.Services.ImageOptimizationService
{
    /// <summary>
    /// Azúcar para los handlers: sube los bytes optimizados cuando la imagen se optimizó,
    /// o el archivo original tal cual cuando no (pass-through). Mantiene la política de
    /// optimización fuera del BlobStorageService.
    /// </summary>
    public static class BlobStorageImageExtensions
    {
        public static Task<string> UploadOptimizedOrOriginalAsync(
            this IBlobStorageService blob,
            IFormFile original,
            ImageOptimizationResult optimization,
            string blobFileName)
        {
            return optimization.WasOptimized
                ? blob.UploadFileAndGetUrlAsync(new MemoryStream(optimization.Data!), blobFileName, optimization.ContentType!)
                : blob.UploadFileAndGetUrlAsync(original, blobFileName);
        }
    }
}
