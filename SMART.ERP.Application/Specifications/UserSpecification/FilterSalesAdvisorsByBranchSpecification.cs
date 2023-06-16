using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.UserSpecification
{
    public class FilterSalesAdvisorsByBranchSpecification : Specification<User>
    {
        public FilterSalesAdvisorsByBranchSpecification(int branchId)
        {
            Query.Where(x => x.Role!.Name == "Sales Advisor" && x.BranchOfficeId == branchId).AsNoTracking();
        }
    }
}
