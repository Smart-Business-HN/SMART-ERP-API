using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Specifications.InventoryExitSpecification
{
    public class FilterAndPaginationInventoryExitSpecification : Specification<InventoryExit>
    {
        public FilterAndPaginationInventoryExitSpecification(string? parameter, int pageNumber, int pageSize,
            InventoryExitReason? reason, InventoryExitStatus? status, int? warehouseId)
        {
            Query.Include(x => x.Warehouse)
                 .OrderByDescending(x => x.Id)
                 .Skip(pageNumber * pageSize)
                 .Take(pageSize)
                 .AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => (x.Code != null && x.Code.Contains(parameter))
                              || (x.BeneficiaryName != null && x.BeneficiaryName.Contains(parameter)));
            }
            if (reason.HasValue)
            {
                Query.Where(x => x.ExitReason == reason.Value);
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
