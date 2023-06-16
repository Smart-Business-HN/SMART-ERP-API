using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class FilterActiveOpportunitiesByCustomerSpecification : Specification<Opportunity>
    {
        public FilterActiveOpportunitiesByCustomerSpecification(Guid? id, bool include)
        {
            Query.Where(x => x.OpportunityStep!.Name != "Ganado" && x.OpportunityStep.Name != "Perdido"
                    && x.OpportunityStep.Name != "Abandonado").AsNoTracking();
            if (include)
            {
                Query.Include(x => x.QuoteProducts!.Where(x => x.IsActive)).ThenInclude(x => x.Product!).ThenInclude(x => x.ProductImages)
                    .Include(x => x.QuoteProducts!.Where(x => x.IsActive)).ThenInclude(x => x.Product!).ThenInclude(x => x.SubCategory)
                    .Include(x => x.OpportunityStep);
            }
            if (id != null)
            {
                Query.Where(x => x.CustomerId == id);
            }
        }
    }
}
