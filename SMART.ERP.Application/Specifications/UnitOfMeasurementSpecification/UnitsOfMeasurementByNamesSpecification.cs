using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.UnitOfMeasurementSpecification
{
    /// <summary>
    /// Carga las unidades de medida cuyo Name esta en la lista dada (resolucion por nombre en lote
    /// para la importacion masiva de productos).
    /// </summary>
    public class UnitsOfMeasurementByNamesSpecification : Specification<UnitOfMeasurement>
    {
        public UnitsOfMeasurementByNamesSpecification(IEnumerable<string> names)
        {
            Query.Where(x => names.Contains(x.Name)).AsNoTracking();
        }
    }
}
