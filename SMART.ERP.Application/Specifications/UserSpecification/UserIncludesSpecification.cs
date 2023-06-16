using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.UserSpecification
{
    public class UserIncludesSpecification : Specification<User>
    {
        public UserIncludesSpecification(Guid? id, string? email)
        {
            Query.Include(x => x.Gender).Include(x => x.Role);
            if (id != null)
            {
                Query.Where(x => x.Id == id);
            }
            if (email != null)
            {
                Query.Where(x => x.Email == email);
            }
        }
    }
}
