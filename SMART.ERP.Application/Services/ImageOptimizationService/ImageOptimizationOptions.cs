namespace SMART.ERP.Application.Services.ImageOptimizationService
{
    /// <summary>
    /// Configuración del servicio de optimización de imágenes. Bindeable desde appsettings
    /// en la sección "ImageOptimization" vía IOptions&lt;T&gt;. Todos los valores tienen defaults sanos,
    /// así que la sección es opcional.
    /// </summary>
    public sealed class ImageOptimizationOptions
    {
        public const string SectionName = "ImageOptimization";

        /// <summary>Calidad WebP (0-100) para imágenes opacas (fotos JPEG/BMP).</summary>
        public int LossyQuality { get; set; } = 80;

        /// <summary>Calidad WebP (0-100) para fuentes que pueden tener transparencia (PNG/GIF/WebP/TIFF).</summary>
        public int AlphaQuality { get; set; } = 90;

        /// <summary>
        /// Tope de dimensión (lado mayor). Default == límite físico de WebP (16383 px),
        /// de modo que en la práctica nunca se reescala y se preserva la resolución original.
        /// </summary>
        public int MaxDimension { get; set; } = 16383;

        /// <summary>Si una imagen ya viene en WebP, por defecto se deja intacta (no se re-codifica).</summary>
        public bool ReEncodeWebp { get; set; } = false;

        /// <summary>Conserva la animación (GIF animado -> WebP animado). Si es false, los animados pasan intactos.</summary>
        public bool PreserveAnimation { get; set; } = true;

        /// <summary>Cota de memoria: archivos por encima de este tamaño no se procesan (pass-through).</summary>
        public long MaxInputBytes { get; set; } = 20 * 1024 * 1024; // 20 MB
    }
}
