using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class FilterOpportunitiesInStepSpecification : Specification<Opportunity>
    {
        public FilterOpportunitiesInStepSpecification(int? opportunityStepId, DateTime? start, DateTime? end, int? branchOfficeId)
        {
            Query.Include(x => x.QuoteProducts!.Where(x => x.IsActive)).ThenInclude(x => x.Product).ThenInclude(x => x!.SubCategory).AsNoTracking();
            if (opportunityStepId != null)
            {
                Query.Where(x => x.OpportunityStepId == opportunityStepId);
            }
            if (start != null)
            {
                Query.Where(x => x.CreationDate >= start);
            }
            if (end != null)
            {
                Query.Where(x => x.CreationDate <= end);
            }
            if (branchOfficeId != null)
            {
                Query.Where(x => x.User!.BranchOfficeId == branchOfficeId);
            }
        }
    }
}
