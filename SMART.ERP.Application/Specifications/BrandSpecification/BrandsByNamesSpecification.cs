using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.BrandSpecification
{
    /// <summary>
    /// Carga las marcas cuyo Name esta en la lista dada (resolucion por nombre en lote para la
    /// importacion masiva de productos).
    /// </summary>
    public class BrandsByNamesSpecification : Specification<Brand>
    {
        public BrandsByNamesSpecification(IEnumerable<string> names)
        {
            Query.Where(x => names.Contains(x.Name)).AsNoTracking();
        }
    }
}
