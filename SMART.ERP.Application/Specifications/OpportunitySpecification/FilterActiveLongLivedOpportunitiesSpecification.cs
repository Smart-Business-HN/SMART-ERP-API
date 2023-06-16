using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class FilterActiveLongLivedOpportunitiesSpecification : Specification<Opportunity>
    {
        public FilterActiveLongLivedOpportunitiesSpecification(int monthsOld, int? branchId)
        {
            Query.Where(x => x.CreationDate <= DateTime.Now.AddMonths(-monthsOld) && x.OpportunityStep!.Name != "Ganado"
             && x.OpportunityStep.Name != "Perdido" && x.OpportunityStep.Name != "Abandonado").AsNoTracking();
            if (branchId != null)
            {
                Query.Where(x => x.User!.BranchOfficeId == branchId);
            }
        }
    }
}
