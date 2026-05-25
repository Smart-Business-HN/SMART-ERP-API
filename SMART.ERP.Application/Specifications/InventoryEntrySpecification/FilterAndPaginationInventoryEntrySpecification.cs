using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Specifications.InventoryEntrySpecification
{
    public class FilterAndPaginationInventoryEntrySpecification : Specification<InventoryEntry>
    {
        public FilterAndPaginationInventoryEntrySpecification(string? parameter, int pageNumber, int pageSize,
            InventoryEntryType? entryType, InventoryEntryStatus? status, int? warehouseId)
        {
            Query.Include(x => x.Warehouse)
                 .Include(x => x.Provider)
                 .OrderByDescending(x => x.Id)
                 .Skip(pageNumber * pageSize)
                 .Take(pageSize)
                 .AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => (x.Code != null && x.Code.Contains(parameter))
                              || (x.SupplierReference != null && x.SupplierReference.Contains(parameter))
                              || (x.Provider != null && x.Provider.Name.Contains(parameter)));
            }
            if (entryType.HasValue)
            {
                Query.Where(x => x.EntryType == entryType.Value);
            }
            if (status.HasValue)
            {
                Query.Where(x => x.Status == status.Value);
            }
            if (warehouseId.HasValue)
            {
                Query.Where(x => x.WarehouseId == warehouseId.Value);
            }
        }
    }
}
