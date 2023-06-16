using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CustomerMachinerySpecification
{
    public class FilterCustomerMachineryByCustomerIdSpecification : Specification<CustomerMachinery>
    {
        public FilterCustomerMachineryByCustomerIdSpecification(Guid id)
        {
            Query.Include(x => x.Product!).ThenInclude(z => z.SubCategory).Where(x => x.CustomerId == id).AsNoTracking();
        }
    }
}
