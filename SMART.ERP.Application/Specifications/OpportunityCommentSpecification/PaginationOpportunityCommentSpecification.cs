using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunityCommentSpecification
{
    public class PaginationOpportunityCommentSpecification : Specification<OpportunityComment>
    {
        public PaginationOpportunityCommentSpecification(string? parameter, int pageNumber, int pageSize,
            string? order, string? column)
        {
            Query.Include(x => x.User).Skip((pageNumber) * pageSize).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.User!.FullName.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "User" ? x.User!.FullName : null);
                }
                else
                {
                    Query.OrderBy(x => column == "User" ? x.User!.FullName : null);
                }
            }
        }
    }
}
