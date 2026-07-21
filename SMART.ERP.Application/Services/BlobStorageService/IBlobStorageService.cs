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

        /// <summary>
        /// Descarga el contenido de un blob a partir de su URL completa. Devuelve
        /// <c>null</c> si la URL no pertenece a nuestra cuenta y contenedor, si el blob
        /// no existe, o si la descarga falla — nunca lanza.
        ///
        /// Va por el SDK de Azure y no por HTTP a propósito: <c>ProductImage.Url</c> es
        /// varchar(max) escribible desde el panel, así que un GET genérico contra ese
        /// valor sería un primitivo de SSRF (p. ej. el endpoint de metadata de la nube)
        /// cuyo resultado terminaría incrustado en un PDF descargable. Además sigue
        /// funcionando si el contenedor deja de ser de lectura pública.
        /// </summary>
        Task<byte[]?> TryDownloadFileByUrlAsync(string blobUrl, CancellationToken ct = default);
    }
}
