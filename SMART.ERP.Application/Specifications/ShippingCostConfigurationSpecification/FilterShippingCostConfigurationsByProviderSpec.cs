using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ShippingCostConfigurationSpecification
{
    public class FilterShippingCostConfigurationsByProviderSpec : Specification<ShippingCostConfiguration>
    {
        public FilterShippingCostConfigurationsByProviderSpec(int providerId)
        {
            Query.Where(x => x.SourceProviderId == providerId)
                .Include(x => x.SourceWarehouse)
                .Include(x => x.SourceProvider)
                .Include(x => x.SourceCity)
                .Include(x => x.DestinationCity)
                .Include(x => x.DestinationDepartment)
                .Include(x => x.Product);
        }
    }
}
