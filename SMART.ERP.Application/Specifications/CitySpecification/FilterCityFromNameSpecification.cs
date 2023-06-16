using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CitySpecification
{
    public class FilterCityFromNameSpecification : Specification<City>
    {
        public FilterCityFromNameSpecification(string name, int? id)
        {
            Query.Where(x => x.Name == name).AsNoTracking();
            if (id != null)
            {
                Query.Where(x => x.Id != id);
            }
        }
    }
}
