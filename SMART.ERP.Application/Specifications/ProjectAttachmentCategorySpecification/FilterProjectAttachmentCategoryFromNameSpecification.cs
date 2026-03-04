using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProjectAttachmentCategorySpecification
{
    public class FilterProjectAttachmentCategoryFromNameSpecification : Specification<ProjectAttachmentCategory>
    {
        public FilterProjectAttachmentCategoryFromNameSpecification(string name)
        {
            Query.Where(x => x.Name == name);
        }
    }
}
