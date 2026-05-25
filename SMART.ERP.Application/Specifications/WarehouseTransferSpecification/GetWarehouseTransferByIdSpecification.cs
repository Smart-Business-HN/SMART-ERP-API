using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.WarehouseTransferSpecification
{
    public class GetWarehouseTransferByIdSpecification : Specification<WarehouseTransfer>
    {
        public GetWarehouseTransferByIdSpecification(int id)
        {
            Query.Where(x => x.Id == id)
                 .Include(x => x.OriginWarehouse)
                 .Include(x => x.DestinationWarehouse)
                 .Include(x => x.Prefix)
                 .Include(x => x.Items!)
                    .ThenInclude(i => i.Product)
                 .AsNoTracking();
        }
    }
}
