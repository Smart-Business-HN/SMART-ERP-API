using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class FilterOpportunityByStepSpecification : Specification<Opportunity>
    {
        public FilterOpportunityByStepSpecification(int stepId, int? branchId, int? previous, int? current, Guid? userId)
        {
            if (current == null && previous == null)
            {
                Query.Where(x => x.OpportunityStepId == stepId);
            }
            if (branchId != null)
            {
                Query.Where(x => x.User!.BranchOfficeId == branchId);
            }
            if (current != null && previous != null)
            {
                if (current > previous)
                {
                    Query.Where(x => x.OpportunityStepId == stepId && (x.Position > previous && x.Position <= current));
                }
                else
                {
                    Query.Where(x => x.OpportunityStepId == stepId && (x.Position >= current && x.Position < previous));
                }
            }
            else if (current != null || previous != null)
            {
                if (previous != null)
                {
                    Query.Where(x => x.OpportunityStepId == stepId && x.Position > previous);
                }
                else
                {
                    Query.Where(x => x.OpportunityStepId == stepId && x.Position >= current);
                }

            }
            if (userId != null)
            {
                Query.Where(x => x.UserId == userId);
            }
        }
    }
}
