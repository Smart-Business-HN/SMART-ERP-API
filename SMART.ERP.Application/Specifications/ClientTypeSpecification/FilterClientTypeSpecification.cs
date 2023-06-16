using Ardalis.Specification;
using SMART.MASTER.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ClientTypeSpecification
{
    public class FilterClientTypeSpecification : Specification<ClientType>
    {
        public FilterClientTypeSpecification(string name)
        {
            Query.Where(x => x.Name == name).AsNoTracking();
        }
    }
}
