using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.UserSpecification
{
    public class FilterUserSpecification : Specification<User>
    {
        public FilterUserSpecification(string filter, Guid? id)
        {
            Query.Include(x => x.Role);
            if (id != null)
                Query.Where(x => x.UserName == filter && x.Id != id || x.FullName == filter && x.Id != id
                || x.Email == filter && x.Id != id || x.PhoneNumber == filter && x.Id != id);
            else
                Query.Where(x => x.UserName == filter || x.FullName == filter
                || x.Email == filter || x.PhoneNumber == filter);
        }
    }
}
