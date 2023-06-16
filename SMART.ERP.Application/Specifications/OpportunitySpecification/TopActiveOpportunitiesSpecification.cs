using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class TopActiveOpportunitiesSpecification : Specification<Opportunity>
    {
        public TopActiveOpportunitiesSpecification(int? branchId)
        {

            Query.Include(x => x.User).Include(x => x.Customer).Include(x => x.InterestLevel)
                .OrderByDescending(x => x.Total).Take(10).Where(x => x.OpportunityStep!.Name != "Ganado" && x.OpportunityStep.Name != "Perdido"
                && x.OpportunityStep.Name != "Abandonado" && x.Total > 0).AsNoTracking();
            if (branchId != null)
            {
                Query.Where(x => x.User!.BranchOfficeId == branchId);
            }
        }
    }
}
