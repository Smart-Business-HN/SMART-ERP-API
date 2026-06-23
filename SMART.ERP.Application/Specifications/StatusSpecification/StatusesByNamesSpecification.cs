using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.StatusSpecification
{
    /// <summary>
    /// Carga los estados cuyo Name esta en la lista dada (resolucion por nombre en lote para la
    /// importacion masiva de productos).
    /// </summary>
    public class StatusesByNamesSpecification : Specification<Status>
    {
        public StatusesByNamesSpecification(IEnumerable<string> names)
        {
            Query.Where(x => names.Contains(x.Name)).AsNoTracking();
        }
    }
}
