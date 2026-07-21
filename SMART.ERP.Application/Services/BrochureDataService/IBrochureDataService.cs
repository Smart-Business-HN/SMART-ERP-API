using SMART.ERP.Application.DTOs.Brochure;

namespace SMART.ERP.Application.Services.BrochureDataService
{
    /// <summary>Topes de escala del brochure.</summary>
    public static class BrochureLimits
    {
        /// <summary>
        /// Tope de productos por brochure (~52 páginas). Más que esto deja de ser
        /// material repartible y se vuelve un PDF que nadie abre en el celular.
        /// </summary>
        public const int MaxProducts = 300;

        /// <summary>Tope por faceta, para que el filtro no arme un IN gigante.</summary>
        public const int MaxBrands = 50;
        public const int MaxCategories = 50;
        public const int MaxSubCategories = 100;
    }

    public interface IBrochureDataService
    {
        /// <summary>Conteos y estimaciones, sin tocar imágenes ni generar PDF.</summary>
        Task<BrochurePreviewDto> GetPreviewAsync(BrochureFilterDto filter, CancellationToken ct = default);

        /// <summary>
        /// Arma el documento completo: productos, precios estrictos de la lista elegida
        /// e imágenes ya normalizadas. Lanza <c>ApiException</c> si los filtros no
        /// producen nada imprimible o si exceden el tope.
        /// </summary>
        Task<BrochureDocumentDto> BuildDocumentAsync(BrochureFilterDto filter, CancellationToken ct = default);
    }
}
