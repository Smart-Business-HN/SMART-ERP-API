using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SMART.ERP.Application.Services.BlobStorageService;

namespace SMART.ERP.Application.Services.BrochureImageService
{
    /// <summary>
    /// Trae las fotos de producto desde Blob Storage y las deja listas para incrustar
    /// en el PDF: reescaladas, aplanadas sobre blanco y en JPEG baseline.
    /// </summary>
    public class BrochureImageService : IBrochureImageService, IDisposable
    {
        /// <summary>
        /// Lado máximo en píxeles. Una tarjeta del grid 2x3 ocupa ~250 pt de ancho;
        /// 560 px da retina de sobra y mantiene el archivo repartible por WhatsApp.
        /// </summary>
        private const int MaxDimension = 560;

        private const int JpegQuality = 75;

        /// <summary>
        /// Descargas simultáneas. Ocho satura la red sin ahogar al servidor, que además
        /// atiende el ecommerce.
        /// </summary>
        private const int DegreeOfParallelism = 8;

        private static readonly TimeSpan PerImageTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Techo para toda la fase de imágenes. Al vencerse se deja de pedir y se sigue
        /// con lo que haya: un brochure con placeholders vale más que un 500.
        /// </summary>
        private static readonly TimeSpan OverallBudget = TimeSpan.FromSeconds(120);

        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(30);

        // Se resuelve IBlobStorageService por ámbito y no por constructor: este servicio
        // se registra como Singleton para que la caché sobreviva entre peticiones, y
        // IBlobStorageService/BlobServiceClient son Transient/Scoped. Inyectarlos directo
        // sería una dependencia cautiva.
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BrochureImageService> _logger;

        /// <summary>
        /// Caché propia y acotada, no la global de la app: son buffers grandes y un
        /// IMemoryCache compartido sin límite sería una fuga a nivel proceso. Se paga
        /// sola porque el usuario suele regenerar el brochure variando un filtro.
        /// </summary>
        private readonly MemoryCache _cache = new(new MemoryCacheOptions
        {
            SizeLimit = 64L * 1024 * 1024 // 64 MB de imágenes ya procesadas
        });

        public BrochureImageService(
            IServiceScopeFactory scopeFactory,
            ILogger<BrochureImageService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task<IReadOnlyDictionary<string, byte[]>> GetImagesAsync(
            IEnumerable<string> imageUrls,
            CancellationToken ct = default)
        {
            // Dedup: varios productos comparten foto (variantes, kits).
            var urls = imageUrls
                .Where(u => !string.IsNullOrWhiteSpace(u))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var result = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
            if (urls.Count == 0) return result;

            using var budgetCts = new CancellationTokenSource(OverallBudget);

            // Un solo ámbito para toda la tanda: BlobServiceClient es thread-safe, así
            // que las 8 descargas paralelas comparten la misma instancia sin problema.
            using var scope = _scopeFactory.CreateScope();
            var blobStorage = scope.ServiceProvider.GetRequiredService<IBlobStorageService>();

            // El presupuesto se enlaza al token POR IMAGEN, nunca a ParallelOptions:
            // si se cancelara el bucle, Parallel.ForEachAsync lanzaría y perderíamos
            // el documento entero en vez de degradar a placeholders.
            await Parallel.ForEachAsync(
                urls,
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = DegreeOfParallelism,
                    CancellationToken = ct // solo el abandono real del cliente
                },
                async (url, loopCt) =>
                {
                    var bytes = await LoadOneAsync(blobStorage, url, budgetCts.Token, loopCt);
                    if (bytes is null) return;

                    lock (result)
                    {
                        result[url] = bytes;
                    }
                });

            _logger.LogInformation(
                "Brochure: {Resolved}/{Total} imágenes resueltas ({Placeholder} con placeholder).",
                result.Count, urls.Count, urls.Count - result.Count);

            return result;
        }

        private async Task<byte[]?> LoadOneAsync(
            IBlobStorageService blobStorage,
            string url,
            CancellationToken budgetToken,
            CancellationToken loopToken)
        {
            if (_cache.TryGetValue(url, out byte[]? cached) && cached is not null)
            {
                return cached;
            }

            // Si ya se agotó el presupuesto global, ni siquiera se intenta.
            if (budgetToken.IsCancellationRequested) return null;

            try
            {
                using var perImage = CancellationTokenSource.CreateLinkedTokenSource(budgetToken, loopToken);
                perImage.CancelAfter(PerImageTimeout);

                var raw = await blobStorage.TryDownloadFileByUrlAsync(url, perImage.Token);
                if (raw is null || raw.Length == 0) return null;

                var processed = Normalize(raw);
                if (processed is null) return null;

                _cache.Set(url, processed, new MemoryCacheEntryOptions
                {
                    Size = processed.Length,
                    AbsoluteExpirationRelativeToNow = CacheTtl
                });

                return processed;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Brochure: no se pudo preparar la imagen {Url}.", url);
                return null;
            }
        }

        /// <summary>
        /// Reescala, aplana sobre blanco y codifica a JPEG baseline.
        ///
        /// El aplanado es obligatorio, no cosmético: JPEG no tiene canal alfa, así que
        /// una PNG/WebP transparente sin aplanar sale con los píxeles transparentes en
        /// NEGRO. Además, QuestPDF codifica en PNG cualquier imagen con alfa e ignora
        /// la calidad de compresión, con lo que el archivo se dispararía de tamaño.
        /// </summary>
        private byte[]? Normalize(byte[] raw)
        {
            try
            {
                using var image = Image.Load(raw);

                image.Mutate(x =>
                {
                    if (image.Width > MaxDimension || image.Height > MaxDimension)
                    {
                        x.Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = new Size(MaxDimension, MaxDimension)
                        });
                    }

                    x.BackgroundColor(Color.White);
                });

                using var output = new MemoryStream();
                image.Save(output, new JpegEncoder
                {
                    Quality = JpegQuality,
                    ColorType = JpegColorType.YCbCrRatio420
                });

                return output.ToArray();
            }
            catch (Exception ex)
            {
                // Formato no soportado o archivo corrupto.
                _logger.LogDebug(ex, "Brochure: imagen ilegible, se usará placeholder.");
                return null;
            }
        }

        public void Dispose() => _cache.Dispose();
    }
}
