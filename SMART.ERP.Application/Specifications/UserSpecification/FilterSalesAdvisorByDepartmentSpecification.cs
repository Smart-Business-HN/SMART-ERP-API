using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.UserSpecification
{
    public class FilterSalesAdvisorByDepartmentSpecification : Specification<User>
    {
        public FilterSalesAdvisorByDepartmentSpecification(int departmentId)
        {
            Query.Where(x => x.Role!.Name == "Sales Advisor" && x.Departments!.Any(x => x.DepartmentId == departmentId)).AsNoTracking();
        }
    }
}
