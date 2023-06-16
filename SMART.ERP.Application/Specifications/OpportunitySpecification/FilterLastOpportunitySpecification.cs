using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class FilterLastOpportunitySpecification : Specification<Opportunity>
    {
        public FilterLastOpportunitySpecification()
        {
            Query.OrderByDescending(x => x.Id).Take(1).AsNoTracking();
        }
    }
}
