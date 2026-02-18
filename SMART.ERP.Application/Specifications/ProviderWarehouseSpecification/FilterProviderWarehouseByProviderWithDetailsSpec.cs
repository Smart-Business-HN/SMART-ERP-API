using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProviderWarehouseSpecification
{
    public class FilterProviderWarehouseByProviderWithDetailsSpec : Specification<ProviderWarehouse>
    {
        public FilterProviderWarehouseByProviderWithDetailsSpec(int providerId)
        {
            Query.Where(pw => pw.ProviderId == providerId)
                 .Include(pw => pw.Provider)
                 .Include(pw => pw.Warehouse);
        }
    }
}
