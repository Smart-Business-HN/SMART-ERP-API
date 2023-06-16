using Ardalis.Specification;
using SMART.MASTER.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ClientSpecification
{
    public class FilterClientFromPhoneSpecification : Specification<Client>
    {
        public FilterClientFromPhoneSpecification(string phone)
        {
            Query.Where(x => x.PhoneNumber == phone).AsNoTracking();
        }
    }
}
