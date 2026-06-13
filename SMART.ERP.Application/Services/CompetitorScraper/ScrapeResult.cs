namespace SMART.ERP.Application.Services.CompetitorScraper
{
    /// <summary>
    /// Resultado de leer el precio de una fuente de competencia. <see cref="Price"/> es el precio crudo
    /// tal como se muestra en el sitio (la normalización de impuestos ocurre aguas arriba).
    /// </summary>
    public sealed class ScrapeResult
    {
        public bool Success { get; init; }
        public decimal? Price { get; init; }
        /// <summary>null = desconocido (se asume disponible); false = explícitamente agotado.</summary>
        public bool? InStock { get; init; }
        public string? Error { get; init; }

        public static ScrapeResult Ok(decimal price, bool? inStock) =>
            new() { Success = true, Price = price, InStock = inStock };

        public static ScrapeResult Fail(string error) =>
            new() { Success = false, Error = error.Length > 500 ? error[..500] : error };
    }
}
