using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.RegionSpecification
{
    public class GetRegionByIdSpecification : Specification<Region>
    {
        public GetRegionByIdSpecification(int? countryId, int? regionId, int? departmentId)
        {
            if (countryId > 0 && regionId == 0)
            {
                Query.Include(x => x.Departments).Where(x => x.CountryId == countryId).AsNoTracking();
            }
            else if (countryId > 0 && regionId > 0 && departmentId == 0)
            {
                Query.Include(x => x.Departments).Where(x => x.CountryId == countryId && x.Id == regionId).AsNoTracking();
            }
            else if (countryId > 0 && regionId > 0 && departmentId > 0)
            {
                Query.Include(x => x.Departments.Where(x => x.Id == departmentId)).Where(x => x.CountryId == countryId && x.Id == regionId).AsNoTracking();
            }
            else
            {
                Query.Include(x => x.Departments);
            }
        }
    }
}
