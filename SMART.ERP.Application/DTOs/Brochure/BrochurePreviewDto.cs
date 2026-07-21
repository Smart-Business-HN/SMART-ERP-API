namespace SMART.ERP.Application.DTOs.Brochure
{
    /// <summary>
    /// Resumen barato (sin descargar imágenes ni generar PDF) para que el usuario
    /// vea qué va a producir antes de disparar una operación de 15-30 segundos.
    /// </summary>
    public class BrochurePreviewDto
    {
        /// <summary>Productos que cumplen los filtros (stock, ecommerce, marca/categoría).</summary>
        public int MatchingProducts { get; set; }

        /// <summary>De esos, los que sí tienen precio en la lista elegida. Son los que se imprimen.</summary>
        public int ProductCount { get; set; }

        /// <summary>
        /// Los que NO tienen fila en la lista elegida. Se excluyen del brochure:
        /// publicar "L. 0.00" o el precio de otra lista es peor que un catálogo más corto.
        /// </summary>
        public int ProductsWithoutPrice { get; set; }

        /// <summary>Se imprimen igual, con placeholder gris.</summary>
        public int ProductsWithoutImage { get; set; }

        public int EstimatedPages { get; set; }

        public bool ExceedsLimit { get; set; }
        public int MaxProducts { get; set; }

        public string? PriceListName { get; set; }

        /// <summary>Etiquetas legibles de los filtros aplicados, para mostrar en la UI y en la portada.</summary>
        public List<string> AppliedFilters { get; set; } = [];
    }
}
