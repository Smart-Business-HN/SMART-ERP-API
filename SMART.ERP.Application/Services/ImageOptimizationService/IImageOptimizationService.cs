using Microsoft.AspNetCore.Http;

namespace SMART.ERP.Application.Services.ImageOptimizationService
{
    public interface IImageOptimizationService
    {
        /// <summary>
        /// Optimiza una imagen raster y la convierte a WebP, preservando resolución, transparencia y
        /// animación. NO lanza por motivos de contenido: si el archivo no es una imagen optimizable,
        /// o si la decodificación/codificación falla, o si el resultado no sería más pequeño, retorna
        /// un resultado de pass-through para que el llamador suba el archivo original tal cual.
        /// </summary>
        Task<ImageOptimizationResult> OptimizeAsync(IFormFile file, CancellationToken cancellationToken = default);

        /// <summary>
        /// Misma optimización que la sobrecarga de <see cref="IFormFile"/>, pero a partir de bytes en
        /// memoria (por ejemplo, un blob descargado en el backfill de imágenes). El <paramref name="fileName"/>
        /// solo se usa para decidir la extensión/optimizabilidad y para el logging.
        /// </summary>
        Task<ImageOptimizationResult> OptimizeAsync(byte[] data, string fileName, CancellationToken cancellationToken = default);

        /// <summary>
        /// True si la extensión corresponde a un formato raster que podemos optimizar
        /// (jpg, jpeg, png, gif, webp, bmp, tiff). Útil para handlers multi-archivo que mezclan
        /// imágenes con PDF/Office/ZIP.
        /// </summary>
        bool IsOptimizableImage(string fileNameOrExtension);
    }
}
