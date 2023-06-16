using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunityActivitySpecification
{
    public class FilterUpcomingActivitiesByUserSpecification : Specification<OpportunityActivity>
    {
        public FilterUpcomingActivitiesByUserSpecification(Guid userId)
        {
            Query.Include(x => x.TypeActivity).Include(x => x.Status).Where(x => x.UserId == userId && x.EndDate > DateTime.Now)
                .OrderBy(a => a.EndDate).Take(10).AsNoTracking();
        }
    }
}
