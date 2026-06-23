using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    /// <summary>
    /// Carga los productos cuyo Name esta en la lista dada, para detectar duplicados contra la BD
    /// durante la importacion masiva. NO filtra IsDeleted: un producto archivado sigue ocupando el
    /// nombre, asi que el import no debe colisionar con el.
    /// </summary>
    public class ProductsByNamesSpecification : Specification<Product>
    {
        public ProductsByNamesSpecification(IEnumerable<string> names)
        {
            Query.Where(x => names.Contains(x.Name)).AsNoTracking();
        }
    }
}
