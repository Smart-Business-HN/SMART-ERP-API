using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ClientSpecification
{
    public class FilterClientByUniqueValues : Specification<Customer>
    {
        public FilterClientByUniqueValues(string fullName, string phone, string? email, Guid? Id)
        {
            if (!string.IsNullOrEmpty(email))
                Query.Where(x => x.Email == email);
            else
                Query.Where(x => x.FullName == fullName || x.PhoneNumber == phone);

            if (Id != null)
            {
                Query.Where(x => x.Id != Id);
            }
        }
    }
}
