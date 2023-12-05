using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProviderSpecification
{
    public class FilterProviderSpecification : Specification<Provider>
    {
        public FilterProviderSpecification(string? filter, int? id)
        {
            if (id != null) Query.Include(x=>x.TypeProvider).Where(x => x.Id != id).AsNoTracking();
            else Query.Where(x => x.Name == filter || x.RTN == filter
            || x.Email == filter || x.PhoneNumber == filter).AsNoTracking();
        }
    }
}
