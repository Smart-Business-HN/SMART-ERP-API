namespace SMART.ERP.Application.DTOs.Brochure
{
    /// <summary>Un producto ya listo para pintarse en una tarjeta del brochure.</summary>
    public class BrochureProductItemDto
    {
        public int ProductId { get; set; }

        /// <summary>SKU.</summary>
        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        /// <summary>Texto plano ya saneado y truncado. Nunca HTML.</summary>
        public string ShortDescription { get; set; } = string.Empty;

        public string? BrandName { get; set; }
        public string? CategoryName { get; set; }
        public string? SubCategoryName { get; set; }

        public string? ImageUrl { get; set; }

        /// <summary>
        /// Imagen ya descargada, reescalada y codificada a JPEG. <c>null</c> significa
        /// que se debe pintar el placeholder (no hubo imagen, o falló la descarga).
        /// </summary>
        public byte[]? ImageBytes { get; set; }

        /// <summary>
        /// Precio que se imprime, tal cual sale de <c>PriceListItem.Price</c>.
        ///
        /// IMPORTANTE: este valor YA INCLUYE ISV. Las listas se generan como
        /// <c>CEILING(CostPrice * Multiplicador * (1 + Tax.Rate/100))</c> —
        /// ver <c>Migrations/20260518050044_AddPriceListSystem.cs</c> línea 155 y
        /// <c>RegeneratePriceListFromCostCommand.cs</c> línea 52.
        /// NUNCA multiplicar por <c>Tax.Rate</c> aquí: publicaría el precio inflado
        /// un 15-18% y dejaría de coincidir con lo que muestra la tienda.
        /// </summary>
        public decimal ListPrice { get; set; }

        /// <summary>Tasa de ISV del producto. Solo informativa/auditoría.</summary>
        public decimal TaxRate { get; set; }
    }
}
