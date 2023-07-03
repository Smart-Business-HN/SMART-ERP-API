using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CustomerSpecification
{
    public class FilterCustomerByMasterIdSpecification : Specification<Customer>
    {
        public FilterCustomerByMasterIdSpecification(Guid id)
        {
            Query.Where(x => x.Id == id).AsNoTracking();
        }
    }
}
