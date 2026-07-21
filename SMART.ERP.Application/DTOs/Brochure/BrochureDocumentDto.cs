namespace SMART.ERP.Application.DTOs.Brochure
{
    /// <summary>
    /// Todo lo que el servicio de PDF necesita para componer el documento.
    /// Llega completamente materializado — precios finales, textos ya saneados,
    /// imágenes ya en memoria — para que el generador no haga nada de I/O.
    /// </summary>
    public class BrochureDocumentDto
    {
        public string Title { get; set; } = "CATÁLOGO DE PRODUCTOS";

        /// <summary>Ej. "Hikvision · Ubiquiti". Vacío si no se filtró por marca.</summary>
        public string? BrandsLabel { get; set; }

        /// <summary>Ej. "Cámaras · Cableado estructurado".</summary>
        public string? CategoriesLabel { get; set; }

        public string PriceListName { get; set; } = null!;

        public DateTime GeneratedAt { get; set; }

        /// <summary>Productos ya agrupables por <c>CategoryName</c> y ordenados.</summary>
        public List<BrochureProductItemDto> Products { get; set; } = [];

        /// <summary>
        /// Cuántas tarjetas van a salir con placeholder. Si es una proporción alta
        /// se imprime una nota en la contraportada — un log no lo ve quien descarga.
        /// </summary>
        public int PlaceholderCount { get; set; }
    }
}
