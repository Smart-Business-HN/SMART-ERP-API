using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CountrySpecification
{
    public class IncludeCountrySpecification : Specification<Country>
    {
        public IncludeCountrySpecification(int? id)
        {
            if (id != null && id > 0)
                Query.Include(a => a.Regions!).ThenInclude(x => x.Departments!)
                    .ThenInclude(x => x.Cities).Where(x => x.Id == id);
            else
                Query.Include(a => a.Regions!).ThenInclude(x => x.Departments!);
        }
    }
}
