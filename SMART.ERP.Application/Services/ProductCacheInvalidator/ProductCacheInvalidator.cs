using Microsoft.AspNetCore.OutputCaching;

namespace SMART.ERP.Application.Services.ProductCacheInvalidator
{
    public class ProductCacheInvalidator : IProductCacheInvalidator
    {
        private static readonly string[] ProductTags =
        [
            "cache_products",
            "cache_productsEcommerce",
            "cache_productsBySameCategorySlug",
            "cache_productsByCategorySlug",
            "cache_productsBySubCategorySlug",
            "cache_productSearch"
        ];

        private readonly IOutputCacheStore _outputCacheStore;

        public ProductCacheInvalidator(IOutputCacheStore outputCacheStore)
        {
            _outputCacheStore = outputCacheStore;
        }

        public async Task InvalidateAsync(CancellationToken cancellationToken = default)
        {
            foreach (var tag in ProductTags)
            {
                await _outputCacheStore.EvictByTagAsync(tag, cancellationToken);
            }
        }
    }
}
