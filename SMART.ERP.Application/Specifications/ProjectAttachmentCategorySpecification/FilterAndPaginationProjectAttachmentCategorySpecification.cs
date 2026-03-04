using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProjectAttachmentCategorySpecification
{
    public class FilterAndPaginationProjectAttachmentCategorySpecification : Specification<ProjectAttachmentCategory>
    {
        public FilterAndPaginationProjectAttachmentCategorySpecification(string? parameter, int pageNumber, int pageSize,
            string? order, string? column)
        {
            Query.Skip(pageNumber * pageSize).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Name.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                    Query.OrderByDescending(x => x.Name);
                else
                    Query.OrderBy(x => x.Name);
            }
        }
    }
}
