using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CustomerSpecification
{
    public class BasicCustomerIncludesSpecification : Specification<Customer>
    {
        public BasicCustomerIncludesSpecification()
        {
            Query.Include(x => x.CustomerType);
        }
    }
}
