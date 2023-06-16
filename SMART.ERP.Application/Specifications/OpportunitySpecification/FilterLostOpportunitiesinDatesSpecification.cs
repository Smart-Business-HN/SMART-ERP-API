using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class FilterLostOpportunitiesinDatesSpecification : Specification<Opportunity>
    {
        public FilterLostOpportunitiesinDatesSpecification(int year, Guid? id, int? branchId)
        {
            Query.Include(x => x.QuoteProducts!.Where(x => x.IsActive))
                    .Where(x => x.ClosingDate != null && x.ClosingDate.Value.Year == year
                    && (x.OpportunityStep!.Name == "Perdido" || x.OpportunityStep.Name == "Abandonado")).AsNoTracking();

            if (id != null)
            {
                Query.Where(x => x.UserId == id);
            }
            if (branchId != null && branchId != 0)
            {
                Query.Where(x => x.User!.BranchOfficeId == branchId);
            }

        }
    }
}
