using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProviderSpecification
{
    public class FilterAndPaginationProviderSpecification : Specification<Provider>
    {
        public FilterAndPaginationProviderSpecification(string? parameter, int pageNumber,
            int pageSize, string? order, string? column)
        {
            Query.Skip((pageNumber) * pageSize).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Name.Contains(parameter)
                || x.RTN.Contains(parameter)
                || x.PhoneNumber.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "Name" ? x.Name
                    : column == "RTN" ? x.RTN
                    : column == "PhoneNumber" ? x.PhoneNumber
                    : null);
                }
                else
                {
                    Query.OrderBy(x => column == "Name" ? x.Name
                    : column == "RTN" ? x.RTN
                    : column == "PhoneNumber" ? x.PhoneNumber
                    : null);
                }
            }
        }
    }
}
