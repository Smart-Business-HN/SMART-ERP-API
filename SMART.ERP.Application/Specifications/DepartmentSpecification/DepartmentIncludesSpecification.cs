using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.DepartmentSpecification
{
    public class DepartmentIncludesSpecification : Specification<Department>
    {
        public DepartmentIncludesSpecification(int? id)
        {
            Query.Include(x => x.Cities).AsNoTracking();
            if (id != null || id > 0)
            {
                Query.Where(x => x.Id == id);
            }
        }
    }
}
