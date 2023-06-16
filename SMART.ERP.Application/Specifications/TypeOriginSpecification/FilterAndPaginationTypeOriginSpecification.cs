using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.TypeOriginSpecification
{
    public class FilterAndPaginationTypeOriginSpecification : Specification<TypeOrigin>
    {
        public FilterAndPaginationTypeOriginSpecification(string? parameter, int pageNumber, int pageSize,
            string? order, string? column)
        {
            Query.Skip((pageNumber) * pageSize).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Name.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "Name" ? x.Name : column == "IsActive" ? x.IsActive : column == "IsProspectOrigin" ? x.IsProspectOrigin : null);
                }
                else
                {
                    Query.OrderBy(x => column == "Name" ? x.Name : column == "IsActive" ? x.IsActive : column == "IsProspectOrigin" ? x.IsProspectOrigin : null);
                }
            }
        }
    }
}
