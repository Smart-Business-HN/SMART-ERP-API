using Microsoft.AspNetCore.OutputCaching;

namespace SMART.ERP.Application.Features.BaseProductFeature
{
    /// <summary>
    /// Evicta todas las tags de OutputCache que dependen del catalogo de productos.
    /// Se usa al crear/editar/eliminar/restaurar un producto para que ni el admin (v1)
    /// ni el ecommerce (v2) sirvan datos obsoletos.
    /// </summary>
    public static class ProductCacheEviction
    {
        private static readonly string[] Tags =
        {
            "cache_products",
            "cache_productsEcommerce",
            "cache_productsByCategorySlug",
            "cache_productsBySubCategorySlug",
            "cache_productsBySameCategorySlug",
            "cache_productSearch",
            "cache_searchSuggestions",
            "cache_getAllNavCategories",
        };

        public static async Task EvictAsync(IOutputCacheStore store, CancellationToken cancellationToken)
        {
            foreach (var tag in Tags)
            {
                await store.EvictByTagAsync(tag, cancellationToken);
            }
        }
    }
}
