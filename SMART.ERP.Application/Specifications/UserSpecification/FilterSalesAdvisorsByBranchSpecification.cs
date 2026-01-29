using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.UserSpecification
{
    public class FilterSalesAdvisorsByBranchSpecification : Specification<User>
    {
        public FilterSalesAdvisorsByBranchSpecification(int branchId)
        {
            if (branchId > 0)
                Query.Where(x => x.Role!.Name == "Sales Advisor" && x.BranchOfficeId == branchId).AsNoTracking();
            else
                Query.Where(x => x.Role!.Name == "Sales Advisor").AsNoTracking();
        }
    }
}
