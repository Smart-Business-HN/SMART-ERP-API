using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ShippingCostConfigurationSpecification
{
    public class FilterAndPaginationShippingCostConfigurationSpec : Specification<ShippingCostConfiguration>
    {
        public FilterAndPaginationShippingCostConfigurationSpec(string? parameter, int pageNumber,
            int pageSize, string? order, string? column)
        {
            Query.Skip((pageNumber) * pageSize).Take(pageSize).AsNoTracking()
                .Include(x => x.SourceWarehouse)
                .Include(x => x.SourceProvider)
                .Include(x => x.SourceCity)
                .Include(x => x.DestinationCity)
                .Include(x => x.DestinationDepartment)
                .Include(x => x.Product);

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.CostType.Contains(parameter) ||
                                 (x.SourceProvider != null && x.SourceProvider.Name.Contains(parameter)) ||
                                 (x.SourceWarehouse != null && x.SourceWarehouse.Name.Contains(parameter)));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "CostType" ? x.CostType :
                                                  column == "DefaultCost" ? x.DefaultCost :
                                                  column == "Priority" ? x.Priority :
                                                  column == "IsActive" ? x.IsActive : null);
                }
                else
                {
                    Query.OrderBy(x => column == "CostType" ? x.CostType :
                                       column == "DefaultCost" ? x.DefaultCost :
                                       column == "Priority" ? x.Priority :
                                       column == "IsActive" ? x.IsActive : null);
                }
            }
            else
            {
                Query.OrderByDescending(x => x.Priority).ThenBy(x => x.DefaultCost);
            }
        }
    }
}
