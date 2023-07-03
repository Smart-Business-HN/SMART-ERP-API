using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ClientSpecification
{
    public class FilterClientFromPhoneSpecification : Specification<Customer>
    {
        public FilterClientFromPhoneSpecification(string phone)
        {
            Query.Where(x => x.PhoneNumber == phone).AsNoTracking();
        }
    }
}
