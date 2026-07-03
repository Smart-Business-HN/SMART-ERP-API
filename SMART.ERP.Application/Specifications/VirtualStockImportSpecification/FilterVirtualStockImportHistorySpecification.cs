using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.VirtualStockImportSpecification
{
    // Historial de importaciones de stock virtual, ordenado por fecha (más reciente primero) y paginado.
    // Si providerId es null o <= 0 se listan las importaciones de todos los proveedores.
    // El CountAsync de Ardalis evalúa solo los criterios (Where), por lo que esta misma spec sirve
    // para obtener el total sin que Skip/Take afecten el conteo.
    public class FilterVirtualStockImportHistorySpecification : Specification<VirtualStockImport>
    {
        public FilterVirtualStockImportHistorySpecification(int? providerId, int pageNumber, int pageSize)
        {
            Query.Include(x => x.Provider)
                 .OrderByDescending(x => x.ImportDate)
                 .ThenByDescending(x => x.Id)
                 .Skip(pageNumber * pageSize)
                 .Take(pageSize)
                 .AsNoTracking();

            if (providerId.HasValue && providerId.Value > 0)
            {
                Query.Where(x => x.ProviderId == providerId.Value);
            }
        }
    }
}
