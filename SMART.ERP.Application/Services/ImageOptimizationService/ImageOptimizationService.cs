using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace SMART.ERP.Application.Services.ImageOptimizationService
{
    /// <summary>
    /// Implementación con SixLabors.ImageSharp. Este es el ÚNICO archivo que referencia la librería
    /// concreta; cambiar a Magick.NET/SkiaSharp solo requiere reescribir esta clase.
    /// </summary>
    public sealed class ImageOptimizationService : IImageOptimizationService
    {
        private const string WebpContentType = "image/webp";
        private const string WebpExtension = ".webp";

        private static readonly HashSet<string> OptimizableExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".tif", ".tiff"
        };

        // Formatos cuyo contenido puede llevar canal alpha (transparencia).
        private static readonly HashSet<string> AlphaCapableFormats = new(StringComparer.OrdinalIgnoreCase)
        {
            "PNG", "GIF", "WEBP", "TIFF", "BMP"
        };

        private readonly ImageOptimizationOptions _options;
        private readonly ILogger<ImageOptimizationService> _logger;

        public ImageOptimizationService(
            IOptions<ImageOptimizationOptions> options,
            ILogger<ImageOptimizationService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public bool IsOptimizableImage(string fileNameOrExtension)
        {
            if (string.IsNullOrWhiteSpace(fileNameOrExtension))
            {
                return false;
            }

            var ext = Path.GetExtension(fileNameOrExtension);
            if (string.IsNullOrEmpty(ext))
            {
                ext = fileNameOrExtension; // el llamador pasó una extensión "pelada" como "png" o ".png"
            }

            if (!ext.StartsWith('.'))
            {
                ext = "." + ext;
            }

            return OptimizableExtensions.Contains(ext);
        }

        public async Task<ImageOptimizationResult> OptimizeAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            // 1. Pass-through temprano por condiciones baratas.
            if (file == null || file.Length == 0)
            {
                return ImageOptimizationResult.PassThrough(file?.Length ?? 0);
            }

            var originalSize = file.Length;

            if (!ShouldTranscode(file.FileName, originalSize))
            {
                return ImageOptimizationResult.PassThrough(originalSize);
            }

            // 2. Leer a bytes (necesitamos el original para el guard "nunca más grande").
            byte[] originalBytes;
            using (var ms = new MemoryStream())
            {
                await using (var input = file.OpenReadStream())
                {
                    await input.CopyToAsync(ms, cancellationToken);
                }
                originalBytes = ms.ToArray();
            }

            return await TranscodeToWebpAsync(originalBytes, file.FileName, originalSize, cancellationToken);
        }

        public async Task<ImageOptimizationResult> OptimizeAsync(byte[] data, string fileName, CancellationToken cancellationToken = default)
        {
            // 1. Pass-through temprano por condiciones baratas.
            if (data == null || data.Length == 0)
            {
                return ImageOptimizationResult.PassThrough(data?.LongLength ?? 0);
            }

            var originalSize = data.LongLength;

            if (!ShouldTranscode(fileName, originalSize))
            {
                return ImageOptimizationResult.PassThrough(originalSize);
            }

            return await TranscodeToWebpAsync(data, fileName ?? string.Empty, originalSize, cancellationToken);
        }

        /// <summary>
        /// Filtros baratos compartidos por ambas sobrecargas: extensión optimizable, tope de tamaño y
        /// "ya es WebP". Devuelve false (pass-through) cuando no vale la pena decodificar.
        /// </summary>
        private bool ShouldTranscode(string? fileName, long originalSize)
        {
            var extension = Path.GetExtension(fileName);

            if (!IsOptimizableImage(extension))
            {
                return false; // PDF, Office, ZIP, etc.
            }

            if (originalSize > _options.MaxInputBytes)
            {
                _logger.LogInformation(
                    "Imagen '{FileName}' ({Size} bytes) excede MaxInputBytes; se deja sin optimizar.",
                    fileName, originalSize);
                return false;
            }

            // Ya es WebP y no queremos re-codificar -> dejar intacto.
            if (string.Equals(extension, WebpExtension, StringComparison.OrdinalIgnoreCase) && !_options.ReEncodeWebp)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Decodifica + procesa + codifica a WebP. Cualquier fallo -> pass-through (nunca lanza por contenido).
        /// </summary>
        private async Task<ImageOptimizationResult> TranscodeToWebpAsync(byte[] originalBytes, string fileName, long originalSize, CancellationToken cancellationToken)
        {
            try
            {
                using var image = Image.Load(originalBytes);

                var formatName = image.Metadata.DecodedImageFormat?.Name;
                var isAnimated = image.Frames.Count > 1;

                // Animación: si no la queremos preservar, dejar el original intacto.
                if (isAnimated && !_options.PreserveAnimation)
                {
                    return ImageOptimizationResult.PassThrough(originalSize);
                }

                // Auto-orientar (aplica el tag EXIF a los píxeles) ANTES de quitar metadata.
                image.Mutate(x => x.AutoOrient());

                // Quitar metadata para reducir peso (EXIF/IPTC/XMP/ICC).
                image.Metadata.ExifProfile = null;
                image.Metadata.IptcProfile = null;
                image.Metadata.XmpProfile = null;
                image.Metadata.IccProfile = null;

                // Tope de dimensión (preserva proporción). Con el default 16383 esto casi nunca ocurre.
                var maxDimension = Math.Min(_options.MaxDimension, 16383);
                if (image.Width > maxDimension || image.Height > maxDimension)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(maxDimension, maxDimension),
                        Mode = ResizeMode.Max
                    }));
                }

                // Elegir calidad. WebP lossy preserva el canal alpha, así que la transparencia se mantiene.
                var mayHaveAlpha = formatName != null && AlphaCapableFormats.Contains(formatName);
                var quality = Math.Clamp(mayHaveAlpha ? _options.AlphaQuality : _options.LossyQuality, 1, 100);

                var encoder = new WebpEncoder
                {
                    FileFormat = WebpFileFormatType.Lossy,
                    Quality = quality
                };

                // Codificar a WebP.
                using var output = new MemoryStream();
                await image.SaveAsWebpAsync(output, encoder, cancellationToken);
                var optimizedBytes = output.ToArray();

                // Guard "nunca más grande": si no ganamos peso, usar el original.
                if (optimizedBytes.LongLength >= originalBytes.LongLength)
                {
                    _logger.LogInformation(
                        "WebP de '{FileName}' no resultó más pequeño ({Optimized} >= {Original}); se usa el original.",
                        fileName, optimizedBytes.LongLength, originalBytes.LongLength);
                    return ImageOptimizationResult.PassThrough(originalSize);
                }

                _logger.LogInformation(
                    "Imagen '{FileName}' optimizada {Original} -> {Optimized} bytes ({Ratio:P0}).",
                    fileName, originalBytes.LongLength, optimizedBytes.LongLength,
                    (double)optimizedBytes.LongLength / originalBytes.LongLength);

                return ImageOptimizationResult.Optimized(optimizedBytes, WebpContentType, WebpExtension, originalSize);
            }
            catch (Exception ex)
            {
                // Contrato: nunca romper una subida por un fallo de optimización.
                _logger.LogWarning(ex, "Fallo optimizando '{FileName}'; se usa el original.", fileName);
                return ImageOptimizationResult.PassThrough(originalSize);
            }
        }
    }
}
