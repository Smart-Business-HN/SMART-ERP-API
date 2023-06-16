using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class FilterOpportunityByUserInDateSpecification : Specification<Opportunity>
    {
        public FilterOpportunityByUserInDateSpecification(DateTime? startDate, DateTime? endDate, Guid userId)
        {
            Query.Include(x => x.User).Include(x => x.Customer).Include(x => x.InterestLevel).Include(x => x.OpportunityStep)
                .OrderBy(x => x.Position).Where(x => x.UserId == userId).AsNoTracking();
            if (startDate != null)
            {
                Query.Where(x => x.CreationDate.Date >= startDate.Value.Date || (x.ClosingDate.HasValue && x.ClosingDate.Value.Date >= startDate.Value.Date));
            }
            if (endDate != null)
            {
                Query.Where(x => x.CreationDate.Date <= endDate.Value.Date || (x.ClosingDate.HasValue && x.ClosingDate.Value.Date <= endDate.Value));
            }
        }
    }
}
