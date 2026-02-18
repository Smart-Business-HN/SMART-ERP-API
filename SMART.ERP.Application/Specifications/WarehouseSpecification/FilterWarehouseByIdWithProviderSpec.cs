using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.WarehouseSpecification
{
    public class FilterWarehouseByIdWithProviderSpec : Specification<Warehouse>
    {
        public FilterWarehouseByIdWithProviderSpec(int warehouseId)
        {
            Query.Where(w => w.Id == warehouseId)
                .Include(w => w.ProviderWarehouses!)
                .ThenInclude(pw => pw.Provider);
        }
    }
}
