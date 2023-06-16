using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.DocumentTypeSpecification
{
    public class PaginationDocumentTypeSpecification : Specification<DocumentType>
    {
        public PaginationDocumentTypeSpecification(string? parameter, int pageNumber, int pageSize,
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
