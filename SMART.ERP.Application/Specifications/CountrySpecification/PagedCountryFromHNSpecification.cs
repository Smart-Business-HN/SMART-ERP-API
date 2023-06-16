using Ardalis.Specification;
using SMART.MASTER.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CountrySpecification
{
    public class PagedCountryFromHNSpecification : Specification<ClientCountry>
    {
        public PagedCountryFromHNSpecification(string? parameter, int pageNumber,
            int pageSize, string? order, string? column)
        {
            Query.Include(a => a.Departments!).ThenInclude(x => x.Cities).Skip(pageNumber * pageSize).Take(pageSize).AsNoTracking();

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
