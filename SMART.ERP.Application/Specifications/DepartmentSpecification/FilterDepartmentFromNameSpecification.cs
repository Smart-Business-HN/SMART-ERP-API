using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.DepartmentSpecification
{
    public class FilterDepartmentFromNameSpecification : Specification<Department>
    {
        public FilterDepartmentFromNameSpecification(string name, int? id)
        {
            Query.Where(x => x.Name == name).AsNoTracking();
            if (id != null)
            {
                Query.Where(x => x.Id != id);
            }
        }
    }
}
