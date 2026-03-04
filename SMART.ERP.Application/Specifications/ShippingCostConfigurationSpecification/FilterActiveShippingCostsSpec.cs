using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ShippingCostConfigurationSpecification
{
    public class FilterActiveShippingCostsSpec : Specification<ShippingCostConfiguration>
    {
        public FilterActiveShippingCostsSpec(
            int? sourceWarehouseId = null,
            int? sourceProviderId = null,
            int? destinationCityId = null)
        {
            Query.Where(c => c.IsActive);

            if (sourceWarehouseId.HasValue)
            {
                Query.Where(c =>
                    c.SourceWarehouseId == sourceWarehouseId.Value ||
                    (c.SourceProviderId == sourceProviderId && !c.SourceWarehouseId.HasValue));
            }
            else if (sourceProviderId.HasValue)
            {
                Query.Where(c => c.SourceProviderId == sourceProviderId);
            }

            if (destinationCityId.HasValue)
            {
                Query.Where(c =>
                    c.DestinationCityId == destinationCityId.Value ||
                    !c.DestinationCityId.HasValue);
            }

            Query.OrderByDescending(c => c.Priority);
        }
    }
}
