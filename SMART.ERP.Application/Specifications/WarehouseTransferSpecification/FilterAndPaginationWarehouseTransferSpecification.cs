using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Specifications.WarehouseTransferSpecification
{
    public class FilterAndPaginationWarehouseTransferSpecification : Specification<WarehouseTransfer>
    {
        public FilterAndPaginationWarehouseTransferSpecification(string? parameter, int pageNumber, int pageSize,
            WarehouseTransferStatus? status, int? originWarehouseId, int? destinationWarehouseId)
        {
            Query.Include(x => x.OriginWarehouse)
                 .Include(x => x.DestinationWarehouse)
                 .OrderByDescending(x => x.Id)
                 .Skip(pageNumber * pageSize)
                 .Take(pageSize)
                 .AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Code != null && x.Code.Contains(parameter));
            }
            if (status.HasValue)
            {
                Query.Where(x => x.Status == status.Value);
            }
            if (originWarehouseId.HasValue)
            {
                Query.Where(x => x.OriginWarehouseId == originWarehouseId.Value);
            }
            if (destinationWarehouseId.HasValue)
            {
                Query.Where(x => x.DestinationWarehouseId == destinationWarehouseId.Value);
            }
        }
    }
}
