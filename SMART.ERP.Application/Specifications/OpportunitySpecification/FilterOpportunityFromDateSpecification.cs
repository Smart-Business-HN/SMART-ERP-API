using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class FilterOpportunityFromDateSpecification : Specification<Opportunity>
    {
        public FilterOpportunityFromDateSpecification(DateTime date, string? time, int? branchId)
        {
            // IgnoreQueryFilters: historico, debe resolver productos eliminados (soft delete).
            Query.IgnoreQueryFilters();
            Query.Include(x => x.QuoteProducts!.Where(x => x.IsActive)).ThenInclude(x => x.Product).ThenInclude(x => x!.SubCategory).AsNoTracking();
            if (time != null)
            {
                if (time.ToLower() == "año")
                {
                    Query.Where(x => x.ClosingDate != null && x.ClosingDate.Value.Year == date.Year);
                    return;
                }
                else if (time.ToLower() == "mes")
                {
                    Query.Where(x => x.ClosingDate != null && x.ClosingDate.Value.Month == date.Month && x.ClosingDate.Value.Year == date.Year);
                    return;
                }
                else if (time.ToLower() == "semana")
                {
                    Query.Where(x => x.ClosingDate != null && x.ClosingDate.Value.Month == date.Month &&
                    x.ClosingDate.Value.Year == date.Year);
                    return;
                }
                else if (time.ToLower() == "dia")
                {
                    Query.Where(x => x.ClosingDate != null && x.ClosingDate.Value.Month == date.Month && x.ClosingDate.Value.Year == date.Year &&
                    x.ClosingDate.Value.DayOfYear == date.DayOfYear);
                    return;
                }
            }
            else
            {
                Query.Where(x => x.ClosingDate != null && x.ClosingDate.Value.Month == date.Month && x.ClosingDate.Value.Year == date.Year);
            }
            if (branchId != null)
            {
                Query.Where(x => x.User!.BranchOfficeId == branchId);
            }
        }
    }
}
