using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProjectAttachmentSpecification
{
    public class ProjectAttachmentIncludesSpecification : Specification<ProjectAttachment>
    {
        public ProjectAttachmentIncludesSpecification(int? id)
        {
            if (id != null)
            {
                Query.Include(x => x.User).Include(x => x.ProjectAttachmentCategory).Where(x => x.Id == id).AsNoTracking();
            }
            else
            {
                Query.Include(x => x.User).Include(x => x.ProjectAttachmentCategory).AsNoTracking();
            }
        }
    }
}
