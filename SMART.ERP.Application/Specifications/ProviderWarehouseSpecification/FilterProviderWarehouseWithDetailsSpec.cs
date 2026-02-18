using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProviderWarehouseSpecification
{
    public class FilterProviderWarehouseWithDetailsSpec : Specification<ProviderWarehouse>
    {
        public FilterProviderWarehouseWithDetailsSpec()
        {
            Query.Include(pw => pw.Provider)
                 .Include(pw => pw.Warehouse)
                 .OrderBy(pw => pw.Provider!.Name);
        }
    }
}
