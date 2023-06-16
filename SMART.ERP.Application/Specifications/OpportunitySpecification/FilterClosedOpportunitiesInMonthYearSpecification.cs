using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class FilterClosedOpportunitiesInMonthYearSpecification : Specification<Opportunity>
    {
        public FilterClosedOpportunitiesInMonthYearSpecification(int month, int year, Guid? userId, int? branchId)
        {
            Query.Include(x => x.QuoteProducts!.Where(x => x.IsActive)).ThenInclude(x => x.Product).Include(x => x.User)
                .Where(x => x.ClosingDate != null && x.ClosingDate.Value.Month == month && x.ClosingDate.Value.Year == year
                && x.OpportunityStep!.Name == "Ganado").AsNoTracking();
            if (userId != null)
            {
                Query.Where(x => x.UserId == userId);
            }
            if (branchId != null)
            {
                Query.Where(x => x.User!.BranchOfficeId == branchId);
            }
        }
    }
}
