using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunityActivitySpecification
{
    public class PaginationOpportunityActivitySpecification : Specification<OpportunityActivity>
    {
        public PaginationOpportunityActivitySpecification(string? parameter, int pageNumber, int pageSize,
            string? order, string? column)
        {
            Query.Include(x => x.TypeActivity).Include(x => x.Status).Include(x => x.User)
                .Skip((pageNumber) * pageSize).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.TypeActivity!.Name.Contains(parameter)
                || x.Status!.Name.Contains(parameter)
                || x.User!.FullName.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "TypeActivity" ? x.TypeActivity!.Name
                    : column == "Status" ? x.Status!.Name : column == "User" ? x.User!.FullName : null);
                }
                else
                {
                    Query.OrderBy(x => column == "TypeActivity" ? x.TypeActivity!.Name
                    : column == "Status" ? x.Status!.Name : column == "User" ? x.User!.FullName : null);
                }
            }
        }
    }
}
