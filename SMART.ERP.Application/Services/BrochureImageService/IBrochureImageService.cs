namespace SMART.ERP.Application.Services.BrochureImageService
{
    public interface IBrochureImageService
    {
        /// <summary>
        /// Descarga y normaliza las imágenes de las URLs dadas, en paralelo acotado.
        /// El diccionario resultante solo contiene las que se resolvieron bien; una URL
        /// ausente significa "pintar placeholder".
        /// </summary>
        Task<IReadOnlyDictionary<string, byte[]>> GetImagesAsync(
            IEnumerable<string> imageUrls,
            CancellationToken ct = default);
    }
}
