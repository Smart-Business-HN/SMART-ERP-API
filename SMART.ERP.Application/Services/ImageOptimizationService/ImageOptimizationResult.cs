namespace SMART.ERP.Application.Services.ImageOptimizationService
{
    /// <summary>
    /// Resultado de optimizar una imagen. Cuando <see cref="WasOptimized"/> es false, el llamador
    /// debe subir el archivo original tal cual (los campos optimizados quedan en null).
    /// </summary>
    public sealed class ImageOptimizationResult
    {
        public bool WasOptimized { get; init; }

        /// <summary>Bytes optimizados (válido solo cuando <see cref="WasOptimized"/> es true).</summary>
        public byte[]? Data { get; init; }

        /// <summary>Content-type resultante, ej. "image/webp".</summary>
        public string? ContentType { get; init; }

        /// <summary>Extensión resultante, ej. ".webp".</summary>
        public string? Extension { get; init; }

        public long OriginalSize { get; init; }

        public long OptimizedSize { get; init; }

        public static ImageOptimizationResult PassThrough(long originalSize) => new()
        {
            WasOptimized = false,
            OriginalSize = originalSize,
            OptimizedSize = originalSize
        };

        public static ImageOptimizationResult Optimized(byte[] data, string contentType, string extension, long originalSize) => new()
        {
            WasOptimized = true,
            Data = data,
            ContentType = contentType,
            Extension = extension,
            OriginalSize = originalSize,
            OptimizedSize = data.LongLength
        };
    }
}
