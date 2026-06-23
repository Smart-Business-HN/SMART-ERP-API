using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    /// <summary>
    /// Carga los productos cuyo Code esta en la lista dada, para detectar duplicados contra la BD
    /// durante la importacion masiva. NO filtra IsDeleted: un producto archivado sigue ocupando el
    /// codigo, asi que el import no debe colisionar con el.
    /// </summary>
    public class ProductsByCodesSpecification : Specification<Product>
    {
        public ProductsByCodesSpecification(IEnumerable<string> codes)
        {
            Query.Where(x => codes.Contains(x.Code)).AsNoTracking();
        }
    }
}
