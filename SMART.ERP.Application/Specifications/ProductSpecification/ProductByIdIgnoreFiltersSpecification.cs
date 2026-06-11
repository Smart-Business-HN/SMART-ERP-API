using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    /// <summary>
    /// Resuelve un producto por Id ignorando el filtro global de soft delete.
    /// Para lecturas historicas/correctness que deben encontrar el producto aunque
    /// este eliminado (Kardex, cancelar/confirmar documentos de inventario existentes).
    /// </summary>
    public class ProductByIdIgnoreFiltersSpecification : Specification<Product>
    {
        public ProductByIdIgnoreFiltersSpecification(int id)
        {
            Query.IgnoreQueryFilters().Where(x => x.Id == id).AsNoTracking();
        }
    }
}
