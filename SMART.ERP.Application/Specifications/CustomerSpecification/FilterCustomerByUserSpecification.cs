using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CustomerSpecification
{
    public class FilterCustomerByUserSpecification : Specification<Customer>
    {
        public FilterCustomerByUserSpecification(Guid id)
        {
            Query.Where(x => x.UserId == id).AsNoTracking();
        }
    }
}
