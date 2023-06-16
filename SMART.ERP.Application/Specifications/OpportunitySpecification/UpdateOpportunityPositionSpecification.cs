using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class UpdateOpportunityPositionSpecification : Specification<Opportunity>
    {
        public UpdateOpportunityPositionSpecification(int stepId, int? branchId, Guid? userId, int? current, int? previous)
        {
            Query.Where(x => x.OpportunityStepId == stepId).OrderBy(x => x.Position);
            if (branchId != null)
            {
                Query.Where(x => x.User!.BranchOfficeId == branchId);
            }
            if (current != null && previous != null)
            {
                if (current > previous)
                {
                    Query.Where(x => (x.Position > previous && x.Position <= current));
                }
                else
                {
                    Query.Where(x => (x.Position >= current && x.Position < previous));
                }
            }
            else if (current != null || previous != null)
            {
                if (previous != null)
                {
                    Query.Where(x => x.Position > previous);
                }
                else
                {
                    Query.Where(x => x.Position >= current);
                }

            }
            if (userId != null)
            {
                Query.Where(x => x.UserId == userId);
            }
        }
    }
}
