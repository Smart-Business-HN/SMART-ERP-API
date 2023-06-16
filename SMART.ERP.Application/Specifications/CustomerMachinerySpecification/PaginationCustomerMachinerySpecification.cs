using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CustomerMachinerySpecification
{
    public class PaginationCustomerMachinerySpecification : Specification<CustomerMachinery>
    {
        public PaginationCustomerMachinerySpecification(int pageNumber, int pageSize)
        {
            Query.Include(x => x.Product).Skip((pageNumber) * pageSize).Take(pageSize).AsNoTracking();
        }
    }
}

