using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.TaxSpecification
{
    /// <summary>
    /// Carga los impuestos cuyo Name esta en la lista dada (resolucion por nombre en lote). Se
    /// devuelve la entidad completa porque la importacion necesita Tax.Rate para el precio por defecto.
    /// </summary>
    public class TaxesByNamesSpecification : Specification<Tax>
    {
        public TaxesByNamesSpecification(IEnumerable<string> names)
        {
            Query.Where(x => names.Contains(x.Name)).AsNoTracking();
        }
    }
}
