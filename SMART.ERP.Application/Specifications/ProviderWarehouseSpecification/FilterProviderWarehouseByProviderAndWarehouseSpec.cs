using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProviderWarehouseSpecification
{
    public class FilterProviderWarehouseByProviderAndWarehouseSpec : Specification<ProviderWarehouse>
    {
        public FilterProviderWarehouseByProviderAndWarehouseSpec(int providerId, int warehouseId)
        {
            Query.Where(pw => pw.ProviderId == providerId && pw.WarehouseId == warehouseId);
        }
    }
}
