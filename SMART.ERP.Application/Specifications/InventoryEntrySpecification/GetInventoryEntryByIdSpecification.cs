using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InventoryEntrySpecification
{
    public class GetInventoryEntryByIdSpecification : Specification<InventoryEntry>
    {
        public GetInventoryEntryByIdSpecification(int id)
        {
            // IgnoreQueryFilters: historico, debe resolver productos eliminados (soft delete).
            Query.IgnoreQueryFilters();
            Query.Where(x => x.Id == id)
                 .Include(x => x.Warehouse)
                 .Include(x => x.Provider)
                 .Include(x => x.Prefix)
                 .Include(x => x.Project)
                 .Include(x => x.Items!)
                    .ThenInclude(i => i.Product)
                 .AsNoTracking();
        }
    }
}
