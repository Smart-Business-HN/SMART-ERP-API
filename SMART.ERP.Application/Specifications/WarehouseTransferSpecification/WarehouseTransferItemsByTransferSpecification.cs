using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.WarehouseTransferSpecification
{
    public class WarehouseTransferItemsByTransferSpecification : Specification<WarehouseTransferItem>
    {
        public WarehouseTransferItemsByTransferSpecification(int warehouseTransferId)
        {
            Query.Where(x => x.WarehouseTransferId == warehouseTransferId).AsNoTracking();
        }
    }
}
