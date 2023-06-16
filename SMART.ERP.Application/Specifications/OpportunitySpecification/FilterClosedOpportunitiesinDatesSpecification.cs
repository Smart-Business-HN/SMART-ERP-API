using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class FilterClosedOpportunitiesinDatesSpecification : Specification<Opportunity>
    {
        public FilterClosedOpportunitiesinDatesSpecification(DateTime? start, DateTime? end, Guid? id, int? reason, int? branchId)
        {
            Query.Include(x => x.QuoteProducts!.Where(x => x.IsActive)).ThenInclude(x => x.Product!).ThenInclude(x => x.SubCategory)
                .Where(x => x.ClosingDate != null).AsNoTracking();
            if (start != null && end != null)
            {
                Query.Where(x => x.ClosingDate >= start && x.ClosingDate <= end);
            }
            if (id != null)
            {
                Query.Where(x => x.UserId == id);
            }
            if (reason != null)
            {
                if (reason == 0)
                {
                    Query.Where(x => x.OpportunityStep!.Name == "Perdido" || x.OpportunityStep.Name == "Abandonado");
                }
                else
                {
                    Query.Where(x => x.OpportunityStep!.Name == "Ganado");
                }
            }
            if (branchId != null)
            {
                Query.Where(x => x.User!.BranchOfficeId == branchId);
            }

        }
    }
}
