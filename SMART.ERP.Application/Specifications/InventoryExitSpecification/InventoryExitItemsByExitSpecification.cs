using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InventoryExitSpecification
{
    public class InventoryExitItemsByExitSpecification : Specification<InventoryExitItem>
    {
        public InventoryExitItemsByExitSpecification(int inventoryExitId)
        {
            Query.Where(x => x.InventoryExitId == inventoryExitId).AsNoTracking();
        }
    }
}
