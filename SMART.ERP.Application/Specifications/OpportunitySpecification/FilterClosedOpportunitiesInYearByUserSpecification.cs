using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class FilterClosedOpportunitiesInYearByUserSpecification : Specification<Opportunity>
    {
        public FilterClosedOpportunitiesInYearByUserSpecification(int year, Guid? id)
        {
            Query.Include(x => x.QuoteProducts!.Where(x => x.IsActive)).ThenInclude(x => x.Product)
                .ThenInclude(x => x!.SubCategory).Include(x => x.OpportunityStep)
                    .Where(x => x.ClosingDate != null && x.ClosingDate.Value.Year == year).AsNoTracking();
            if (id != null)
            {
                Query.Where(x => x.UserId == id);
            }
        }
    }
}
