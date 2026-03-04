using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProjectAttachmentSpecification
{
    public class PaginationProjectAttachmentSpecification : Specification<ProjectAttachment>
    {
        public PaginationProjectAttachmentSpecification(int projectId, string? parameter, int pageNumber, int pageSize,
            string? order, string? column)
        {
            Query.Include(x => x.ProjectAttachmentCategory).Include(x => x.User)
                .Where(x => x.ProjectId == projectId)
                .Skip(pageNumber * pageSize).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.User!.FullName.Contains(parameter)
                || x.ProjectAttachmentCategory!.Name.Contains(parameter)
                || x.Name.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "User" ? x.User!.FullName
                    : column == "Category" ? x.ProjectAttachmentCategory!.Name
                    : column == "Name" ? x.Name : null);
                }
                else
                {
                    Query.OrderBy(x => column == "User" ? x.User!.FullName
                    : column == "Category" ? x.ProjectAttachmentCategory!.Name
                    : column == "Name" ? x.Name : null);
                }
            }
        }
    }
}
