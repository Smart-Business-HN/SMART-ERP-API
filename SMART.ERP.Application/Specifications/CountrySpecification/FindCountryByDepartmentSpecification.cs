using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CountrySpecification
{
    public class FindCountryByDepartmentSpecification : Specification<Country>
    {
        public FindCountryByDepartmentSpecification(int departmentId)
        {
            Query.Where(x => x.Regions!.Any(y => y.Departments!.Any(y => y.Id == departmentId))).AsNoTracking();
        }
    }
}
