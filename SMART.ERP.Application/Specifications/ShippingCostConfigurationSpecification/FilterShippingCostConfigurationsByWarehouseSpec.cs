using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ShippingCostConfigurationSpecification
{
    public class FilterShippingCostConfigurationsByWarehouseSpec : Specification<ShippingCostConfiguration>
    {
        public FilterShippingCostConfigurationsByWarehouseSpec(int warehouseId)
        {
            Query.Where(x => x.SourceWarehouseId == warehouseId)
                .Include(x => x.SourceWarehouse)
                .Include(x => x.SourceProvider)
                .Include(x => x.SourceCity)
                .Include(x => x.DestinationCity)
                .Include(x => x.DestinationDepartment)
                .Include(x => x.Product);
        }
    }
}
