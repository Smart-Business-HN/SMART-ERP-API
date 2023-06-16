using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class FilterClosedOpportunitiesInYearByBranchSpecification : Specification<Opportunity>
    {
        public FilterClosedOpportunitiesInYearByBranchSpecification(int year, int branchId, bool won)
        {
            Query.Include(x => x.OpportunityStep).Include(x => x.QuoteProducts!.Where(x => x.IsActive))
                .ThenInclude(x => x.Product).Where(x => x.ClosingDate != null && x.ClosingDate.Value.Year == year && x.User!.BranchOfficeId == branchId).AsNoTracking();
            if (won)
            {
                Query.Where(x => x.OpportunityStep.Name == "Ganado");
            }
        }
    }
}
