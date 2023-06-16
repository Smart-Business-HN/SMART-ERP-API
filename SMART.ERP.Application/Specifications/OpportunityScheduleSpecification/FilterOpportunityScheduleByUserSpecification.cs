using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunityScheduleSpecification
{
    public class FilterOpportunityScheduleByUserSpecification : Specification<OpportunitySchedules>
    {
        public FilterOpportunityScheduleByUserSpecification(Guid userId)
        {
            Query.Where(x => x.UserId == userId);
        }
    }
}
