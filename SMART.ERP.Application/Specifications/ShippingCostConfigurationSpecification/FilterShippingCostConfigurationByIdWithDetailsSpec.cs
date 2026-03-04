using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ShippingCostConfigurationSpecification
{
    public class FilterShippingCostConfigurationByIdWithDetailsSpec : Specification<ShippingCostConfiguration>
    {
        public FilterShippingCostConfigurationByIdWithDetailsSpec(int id)
        {
            Query.Where(x => x.Id == id)
                .Include(x => x.SourceWarehouse)
                .Include(x => x.SourceProvider)
                .Include(x => x.SourceCity)
                .Include(x => x.DestinationCity)
                .Include(x => x.DestinationDepartment)
                .Include(x => x.Product);
        }
    }
}
