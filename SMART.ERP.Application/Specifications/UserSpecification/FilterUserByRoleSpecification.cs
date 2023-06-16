using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.UserSpecification
{
    public class FilterUserByRoleSpecification : Specification<User>
    {
        public FilterUserByRoleSpecification(string name, int? branchId)
        {
            if (branchId == null)
            {
                Query.Where(x => x.Role!.Name == name);
            }
            else
            {
                Query.Include(x => x.BranchOffice).Where(x => x.Role!.Name == name && x.BranchOfficeId == branchId);
            }

        }
    }
}
