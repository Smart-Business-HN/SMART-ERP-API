using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class FilterActiveOpportunitiesWithStepSpecification : Specification<Opportunity>
    {
        public FilterActiveOpportunitiesWithStepSpecification()
        {
            Query.Include(x => x.OpportunityStep)
                .Where(x => x.ClosingDate == null && x.IsActive)
                .AsNoTracking();
        }
    }
}
