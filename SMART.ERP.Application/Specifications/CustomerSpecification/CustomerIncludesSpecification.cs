using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CustomerSpecification
{
    public class CustomerIncludesSpecification : Specification<Customer>
    {
        public CustomerIncludesSpecification(string? parameter, int pageNumber,
            int pageSize, string? order, string? column)
        {
            Query.Include(x => x.CustomerType).Skip((pageNumber) * pageSize).Take(pageSize).OrderByDescending(x => x.Id).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.FullName.Contains(parameter)); ;
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "FullName" ? x.FullName : null);
                }
                else
                {
                    Query.OrderBy(x => column == "FullName" ? x.FullName : null);
                }
            }
        }
    }
}
