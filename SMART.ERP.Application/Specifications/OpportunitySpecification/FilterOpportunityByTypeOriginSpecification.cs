using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class FilterOpportunityByTypeOriginSpecification : Specification<Opportunity>
    {
        public FilterOpportunityByTypeOriginSpecification(int originId, int? branchId)
        {
            Query.Include(x => x.TypeOrigin).Where(x => x.TypeOriginId != null && x.TypeOriginId == originId).AsNoTracking();
            if (branchId != null)
            {
                Query.Where(x => x.User!.BranchOfficeId == branchId);
            }
        }
    }
}
