using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    public class FilterAndPaginationProductSpecification : Specification<Product>
    {
        public FilterAndPaginationProductSpecification(string? parameter, int pageNumber, int pageSize,
            string? order, string? column)
        {
            Query.Include(x => x.SubCategory).Include(x => x.Status).Include(x => x.Brand).Include(x => x.Provider).Include(x => x.Tax)
                .Include(x => x.ProductImages).Include(x => x.ProductDataSheets!).ThenInclude(x => x.DataSheet)
                .Include(x => x.InventoryDistributions)!.ThenInclude(x=>x.Warehouse)
                .Skip((pageNumber) * pageSize).Take(pageSize).AsNoTracking();

            // Búsqueda multi-término con relevancia, insensible a mayúsculas y acentos.
            ProductSearchPredicate.Apply(Query, parameter, useEcommerceFields: false);

            // Relevancia por defecto al buscar; respeta el orden por columna (header) del admin.
            ProductSearchPredicate.ApplyOrdering(Query, parameter, legacyOrder: order, legacyColumn: column);
        }
    }
}
