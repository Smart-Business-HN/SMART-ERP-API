using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.DepartmentSpecification
{
    public class FilterListDepartmentSpecification : Specification<Department>
    {
        public FilterListDepartmentSpecification(List<int> departments)
        {
            Query.Where(x => departments.Any(y => y == x.Id));
        }
    }
}
