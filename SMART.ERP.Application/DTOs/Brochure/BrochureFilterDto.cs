namespace SMART.ERP.Application.DTOs.Brochure
{
    /// <summary>
    /// Filtros que el usuario elige en el panel. Las tres facetas son multi-selección;
    /// la lista de precios es única y obligatoria.
    /// </summary>
    public class BrochureFilterDto
    {
        public List<int> BrandIds { get; set; } = [];
        public List<int> CategoryIds { get; set; } = [];
        public List<int> SubCategoryIds { get; set; } = [];

        public int PriceListId { get; set; }

        /// <summary>Título opcional de la portada. Si viene vacío se usa el genérico.</summary>
        public string? Title { get; set; }
    }
}
