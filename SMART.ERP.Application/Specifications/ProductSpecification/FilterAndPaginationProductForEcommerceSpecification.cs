using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    public sealed class FilterAndPaginationProductForEcommerceSpecification : Specification<Product>
    {
        public FilterAndPaginationProductForEcommerceSpecification(string? parameter, int pageNumber, int pageSize,
            string? order, string? column)
        {
            Query.Include(x => x.SubCategory)
                .Include(x => x.Status)
                .Include(x => x.Brand)
                .Include(x => x.ProductImages)
                .Include(x => x.Tax)
                .Skip((pageNumber) * pageSize).Take(pageSize).Where(x => x.ShowInEcommerce).AsNoTracking();

            // Búsqueda multi-término con relevancia, insensible a mayúsculas y acentos.
            ProductSearchPredicate.Apply(Query, parameter, useEcommerceFields: true);

            // Relevancia por defecto al buscar; respeta el orden por columna explícito.
            ProductSearchPredicate.ApplyOrdering(Query, parameter, useEcommerceFields: true, legacyOrder: order, legacyColumn: column);
        }
    }
}
