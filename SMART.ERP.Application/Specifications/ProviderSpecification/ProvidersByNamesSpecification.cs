using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProviderSpecification
{
    /// <summary>
    /// Carga los proveedores cuyo Name esta en la lista dada (resolucion por nombre en lote para la
    /// importacion masiva de productos).
    /// </summary>
    public class ProvidersByNamesSpecification : Specification<Provider>
    {
        public ProvidersByNamesSpecification(IEnumerable<string> names)
        {
            Query.Where(x => names.Contains(x.Name)).AsNoTracking();
        }
    }
}
