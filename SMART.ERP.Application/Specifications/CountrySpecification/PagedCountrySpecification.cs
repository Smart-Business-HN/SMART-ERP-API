using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CountrySpecification
{
    public class PagedCountrySpecification : Specification<Country>
    {
        public PagedCountrySpecification(string? parameter, int pageNumber,
            int pageSize, string? order, string? column, bool includeCities)
        {
            if (includeCities)
                Query.Include(x => x.Departments)!.Include(a => a.Regions)!.ThenInclude(x => x.Departments)!.ThenInclude(x => x.Cities)
                    .Skip((pageNumber) * pageSize).Take(pageSize).AsNoTracking();
            else
                Query.Include(a => a.Regions)!.ThenInclude(x => x.Departments).Skip((pageNumber) * pageSize).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Name.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "Name" ? x.Name : null);
                }
                else
                {
                    Query.OrderBy(x => column == "Name" ? x.Name : null);
                }
            }
        }
    }
}
