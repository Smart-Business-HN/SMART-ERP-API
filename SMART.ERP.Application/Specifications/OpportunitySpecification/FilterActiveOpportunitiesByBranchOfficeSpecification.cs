using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class FilterActiveOpportunitiesByBranchOfficeSpecification : Specification<Opportunity>
    {
        public FilterActiveOpportunitiesByBranchOfficeSpecification(int branchId)
        {
            Query.Include(x => x.OpportunityActivities).Include(x => x.User)
                .Where(x => x.OpportunityActivities != null && x.ClosingDate == null && x.User!.BranchOfficeId == branchId).AsNoTracking();
        }
    }
}
