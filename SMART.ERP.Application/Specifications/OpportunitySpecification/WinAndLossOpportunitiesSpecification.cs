using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class WinAndLossOpportunitiesSpecification : Specification<Opportunity>
    {
        public WinAndLossOpportunitiesSpecification(string type, bool compare, bool compareReason, int branchId)
        {
            Query.Where(x => x.User!.BranchOfficeId == branchId).AsNoTracking();
            var date = DateTime.Now;
            if (type == "Win")
            {
                if (compare) date = date.AddMonths(-1);
                if (compareReason)
                {
                    Query.Include(x => x.OpportunityStep)
                    .Include(x => x.QuoteProducts!.Where(x => x.IsActive))
                    .Include(x => x.WinReason).Where(
                    a => a.OpportunityStep!.Name == "Ganado" && a.ClosingDate.HasValue
                    && a.ClosingDate.Value.Month == date.Month
                    && a.ClosingDate.Value.Year == date.Year);
                }
                else
                {
                    Query.Include(x => x.OpportunityStep)
                        .Include(x => x.QuoteProducts!.Where(x => x.IsActive)).Where(
                        a => a.OpportunityStep!.Name == "Ganado" && a.ClosingDate.HasValue
                        && a.ClosingDate.Value.Month == date.Month
                        && a.ClosingDate.Value.Year == date.Year);
                }
            }
            else if (type == "Loss")
            {
                if (compare) date = date.AddMonths(-1);
                if (compareReason)
                {
                    Query.Include(x => x.OpportunityStep)
                        .Include(x => x.QuoteProducts!.Where(x => x.IsActive))
                        .Include(x => x.LossReason).Where(
                        a => (a.OpportunityStep!.Name == "Perdido" && a.ClosingDate.HasValue
                        && a.ClosingDate.Value.Month == date.Month
                        && a.ClosingDate.Value.Year == date.Year)
                        || (a.OpportunityStep.Name == "Abandonado" && a.ClosingDate.HasValue
                        && a.ClosingDate.Value.Month == date.Month
                        && a.ClosingDate.Value.Year == date.Year));
                }
                else
                {
                    Query.Include(x => x.OpportunityStep)
                        .Include(x => x.QuoteProducts!.Where(x => x.IsActive)).Where(
                        a => (a.OpportunityStep!.Name == "Perdido" && a.ClosingDate.HasValue
                        && a.ClosingDate.Value.Month == date.Month
                        && a.ClosingDate.Value.Year == date.Year)
                        || (a.OpportunityStep.Name == "Abandonado" && a.ClosingDate.HasValue
                        && a.ClosingDate.Value.Month == date.Month
                        && a.ClosingDate.Value.Year == date.Year));
                }
            }
        }
    }
}
