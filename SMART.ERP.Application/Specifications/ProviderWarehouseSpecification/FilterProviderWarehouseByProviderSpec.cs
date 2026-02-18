using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProviderWarehouseSpecification
{
    public class FilterProviderWarehouseByProviderSpec : Specification<ProviderWarehouse>
    {
        public FilterProviderWarehouseByProviderSpec(int providerId)
        {
            Query.Where(pw => pw.ProviderId == providerId && pw.IsActive);
        }
    }
}
