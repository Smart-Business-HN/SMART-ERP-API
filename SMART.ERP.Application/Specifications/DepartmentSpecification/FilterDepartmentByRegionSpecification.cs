using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.DepartmentSpecification
{
    public class FilterDepartmentByRegionSpecification : Specification<Department>
    {
        public FilterDepartmentByRegionSpecification(int regionId)
        {
            Query.Where(x => x.RegionId == regionId);
        }
    }
}
