using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InventoryEntrySpecification
{
    public class InventoryEntryItemsByEntrySpecification : Specification<InventoryEntryItem>
    {
        public InventoryEntryItemsByEntrySpecification(int inventoryEntryId)
        {
            Query.Where(x => x.InventoryEntryId == inventoryEntryId).AsNoTracking();
        }
    }
}
