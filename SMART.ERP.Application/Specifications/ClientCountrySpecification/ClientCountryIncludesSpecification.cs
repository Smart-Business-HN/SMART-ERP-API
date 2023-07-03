using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ClientCountrySpecification
{
    public class ClientCountryIncludesSpecification : Specification<Country>
    {
        public ClientCountryIncludesSpecification()
        {
            Query.Include(x => x.Departments!)
                .ThenInclude(x => x.Cities).AsNoTracking();
        }
    }
}
