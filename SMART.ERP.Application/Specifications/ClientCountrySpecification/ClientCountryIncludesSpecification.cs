using Ardalis.Specification;
using SMART.MASTER.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ClientCountrySpecification
{
    public class ClientCountryIncludesSpecification : Specification<ClientCountry>
    {
        public ClientCountryIncludesSpecification()
        {
            Query.Include(x => x.Departments!)
                .ThenInclude(x => x.Cities).AsNoTracking();
        }
    }
}
