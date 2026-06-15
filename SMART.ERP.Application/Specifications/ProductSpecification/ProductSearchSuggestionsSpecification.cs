using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    /// <summary>
    /// Sugerencias para el typeahead: una sola query (multi-término, busca también por Code),
    /// ordenada por relevancia e insensible a mayúsculas y acentos.
    /// </summary>
    public sealed class ProductSearchSuggestionsSpecification : Specification<Product>
    {
        public ProductSearchSuggestionsSpecification(string searchTerm, int limit)
        {
            Query.Where(x => x.ShowInEcommerce && x.IsActive)
                .Include(x => x.Brand)
                .Include(x => x.ProductImages)
                .AsNoTracking();

            ProductSearchPredicate.Apply(Query, searchTerm, useEcommerceFields: true);
            ProductSearchPredicate.ApplyOrdering(Query, searchTerm, sortBy: "relevance", useEcommerceFields: true);

            Query.Take(limit);
        }
    }
}
