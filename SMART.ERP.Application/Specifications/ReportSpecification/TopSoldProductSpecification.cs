using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ReportSpecification
{
    public class TopSoldProductSpecification : Specification<Opportunity>
    {
        public TopSoldProductSpecification(DateTime? start, DateTime? end, int? branchOfficeId)
        {
            Query.Include(x => x.QuoteProducts!.Where(x => x.IsActive)).ThenInclude(x => x.Product).ThenInclude(x => x!.SubCategory)
                .Where(x => x.OpportunityStep.Name == "Ganado").AsNoTracking();
            if (start != null)
            {
                Query.Where(x => x.ClosingDate >= start);
            }
            if (end != null)
            {
                Query.Where(x => x.ClosingDate <= end);
            }
            if (branchOfficeId != null)
            {
                Query.Where(x => x.User!.BranchOfficeId == branchOfficeId);
            }
        }
    }
}
