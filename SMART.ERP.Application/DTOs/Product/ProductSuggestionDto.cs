namespace SMART.ERP.Application.DTOs.Product
{
    /// <summary>
    /// Sugerencia ligera para el autocompletado/typeahead de productos. Incluye lo mínimo
    /// para mostrar y navegar a un producto desde un dropdown.
    /// </summary>
    public class ProductSuggestionDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Thumbnail { get; set; }
        public string? BrandName { get; set; }
        public decimal Price { get; set; }
    }
}
