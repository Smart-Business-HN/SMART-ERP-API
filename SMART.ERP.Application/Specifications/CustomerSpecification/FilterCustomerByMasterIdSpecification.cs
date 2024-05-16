using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CustomerSpecification
{
    public class FilterCustomerByMasterIdSpecification : Specification<Customer>
    {
        public FilterCustomerByMasterIdSpecification(Guid id)
        {
            Query.Include(x => x.DeliveryDirections).Where(x => x.Id == id).AsNoTracking();
        }
    }
}
