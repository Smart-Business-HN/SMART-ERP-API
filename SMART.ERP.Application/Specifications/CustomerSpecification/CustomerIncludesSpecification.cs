using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CustomerSpecification
{
    public class CustomerIncludesSpecification : Specification<Customer>
    {
        public CustomerIncludesSpecification()
        {
            Query.Include(x => x.CustomerMachinery).AsNoTracking();
        }
    }
}
