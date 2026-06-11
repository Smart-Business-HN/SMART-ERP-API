using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class FilterWonOpportunitiesSpecification : Specification<Opportunity>
    {
        public FilterWonOpportunitiesSpecification(Guid? customer)
        {
            // IgnoreQueryFilters: historico, debe resolver productos eliminados (soft delete).
            Query.IgnoreQueryFilters();
            Query.Include(x => x.QuoteProducts)!.ThenInclude(x => x.Product).ThenInclude(x => x!.SubCategory).Where(x => x.OpportunityStep!.Name == "Ganado");
            if (customer != null)
            {
                Query.Where(x => x.CustomerId == customer);
            }
        }
    }
}
