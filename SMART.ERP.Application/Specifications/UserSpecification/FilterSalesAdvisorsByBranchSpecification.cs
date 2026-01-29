using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.UserSpecification
{
    public class FilterSalesAdvisorsByBranchSpecification : Specification<User>
    {
        public FilterSalesAdvisorsByBranchSpecification(int branchId)
        {
            if (branchId > 0)
                Query.Where(x => x.Role!.Name == "Sales Advisor" && x.BranchOfficeId == branchId)
                    .Include(x => x.Role)
                    .AsNoTracking();
            else
                Query.Where(x => x.Role!.Name == "Sales Advisor")
                    .Include(x => x.Role)
                    .AsNoTracking();
        }
    }
}
