using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.RegionSpecification
{
    public class FilterRegionFromNameSpecification : Specification<Region>
    {
        public FilterRegionFromNameSpecification(string name, int? countryId, int? id)
        {
            Query.Where(x => x.Name == name);
            if (countryId != null)
            {
                Query.Where(x => x.CountryId == countryId);
            }
            if (id != null)
            {
                Query.Where(x => x.Id != id);
            }
        }
    }
}
