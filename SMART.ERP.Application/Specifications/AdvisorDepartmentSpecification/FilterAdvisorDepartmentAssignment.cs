using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.AdvisorDepartmentSpecification
{
    public class FilterAdvisorDepartmentAssignment : Specification<AdvisorDepartment>
    {
        public FilterAdvisorDepartmentAssignment(int departmentId, Guid userId)
        {
            Query.Where(x => x.UserId == userId && x.DepartmentId == departmentId).AsNoTracking();
        }
    }
}
