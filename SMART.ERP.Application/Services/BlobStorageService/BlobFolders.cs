namespace SMART.ERP.Application.Services.BlobStorageService
{
    /// <summary>
    /// Taxonomía de carpetas (prefijos virtuales) dentro del contenedor de blobs. Compartida por la
    /// carga "going-forward" y por la herramienta de backfill, para que ambos organicen los assets igual.
    /// </summary>
    public static class BlobFolders
    {
        public const string Products = "products";
        public const string Categories = "categories";
        public const string Brands = "brands";
        public const string Banners = "banners";
        public const string Avatars = "avatars";
        public const string Notifications = "notifications";
        public const string Attachments = "attachments";
        public const string Misc = "misc";

        public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Products, Categories, Brands, Banners, Avatars, Notifications, Attachments, Misc
        };

        public static bool IsValid(string? folder) =>
            !string.IsNullOrWhiteSpace(folder) && All.Contains(folder);

        /// <summary>Normaliza a minúsculas si es una carpeta válida; de lo contrario cae en <see cref="Misc"/>.</summary>
        public static string NormalizeOrDefault(string? folder) =>
            IsValid(folder) ? folder!.ToLowerInvariant() : Misc;
    }
}
