using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.RegionSpecification
{
    public class FilterAndPaginationRegionSpecification : Specification<Region>
    {
        public FilterAndPaginationRegionSpecification(string? parameter, string? order, string? column)
        {
            Query.Include(x => x.Departments).AsNoTracking();
            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Name.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "Name" ? x.Name
                    : column == "CountryId" ? x.CountryId : null);
                }
                else
                {
                    Query.OrderBy(x => column == "Name" ? x.Name
                    : column == "CountryId" ? x.CountryId : null);
                }
            }
        }
    }
}
