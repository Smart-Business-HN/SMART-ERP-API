using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProspectSpecification
{
    public class FilterProspectByUniqueFieldsSpecification : Specification<Prospect>
    {
        public FilterProspectByUniqueFieldsSpecification(string? fullName, string? phoneNumber, string? email, Guid? id)
        {
            Query.Include(x => x.ProspectStep).Where(x => x.FullName == fullName || x.PhoneNumber == phoneNumber);
            if (id != null)
            {
                Query.Where(x => x.Id != id);
            }
            if (email != null)
            {
                Query.Where(x => x.FullName == fullName || x.Email == email || x.PhoneNumber == phoneNumber);
            }
        }
    }
}
