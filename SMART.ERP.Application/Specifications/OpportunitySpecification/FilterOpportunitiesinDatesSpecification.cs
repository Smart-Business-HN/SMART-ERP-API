using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class FilterOpportunitiesinDatesSpecification : Specification<Opportunity>
    {
        public FilterOpportunitiesinDatesSpecification(DateTime? start, DateTime? end, int? branchId)
        {
            Query.Include(x => x.QuoteProducts!.Where(x => x.IsActive)).ThenInclude(x => x.Product)
                .Include(x => x.OpportunityStep)
                .Include(x => x.Customer).Include(x => x.TypeOrigin).Include(x => x.User).Include(x => x.InterestLevel).Include(x => x.WinReason)
                .Include(x => x.LossReason).OrderBy(x => x.Position).AsNoTracking();

            if (start != null && end != null)
            {
                Query.Where(x => (x.CreationDate.Date >= start.Value.Date && x.CreationDate.Date <= end.Value.Date) || (x.ClosingDate.HasValue &&
                x.ClosingDate.Value.Date >= start.Value.Date && x.ClosingDate.Value.Date <= end.Value.Date));
            }
            if (branchId != null && branchId != 0)
            {
                Query.Where(x => x.User!.BranchOfficeId == branchId);
            }

        }
    }
}
