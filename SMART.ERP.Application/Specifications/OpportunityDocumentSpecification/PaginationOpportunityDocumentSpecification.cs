using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunityDocumentSpecification
{
    public class PaginationOpportunityDocumentSpecification : Specification<OpportunityDocument>
    {
        public PaginationOpportunityDocumentSpecification(string? parameter, int pageNumber, int pageSize,
            string? order, string? column)
        {
            Query.Include(x => x.DocumentType).Include(x => x.User).Skip((pageNumber) * pageSize).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.User!.FullName.Contains(parameter)
                || x.DocumentType!.Name.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "User" ? x.User!.FullName
                    : column == "DocumentType" ? x.DocumentType!.Name : null);
                }
                else
                {
                    Query.OrderBy(x => column == "User" ? x.User!.FullName
                    : column == "DocumentType" ? x.DocumentType!.Name : null);
                }
            }
        }
    }
}
