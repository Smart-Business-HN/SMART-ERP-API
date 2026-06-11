using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class QueryOpportunitySpecification : Specification<Opportunity>
    {
        public QueryOpportunitySpecification(string? parameter, int pageNumber,
            int pageSize, string? order, string? column)
        {
            // IgnoreQueryFilters: historico, debe resolver productos eliminados (soft delete).
            Query.IgnoreQueryFilters();
            Query.Include(x => x.QuoteProducts!.Where(x => x.IsActive)).ThenInclude(x => x.Product).ThenInclude(x => x!.Brand)
                .Include(x => x.QuoteProducts!.Where(x => x.IsActive)).ThenInclude(x => x.Product)
                .Include(x => x.QuoteProducts!.Where(x => x.IsActive)).ThenInclude(x => x.Product).ThenInclude(x => x!.SubCategory)
                .Include(x => x.InterestLevel).Include(x => x.User)
                .Include(x => x.Customer).Include(x => x.TypeOrigin).Skip((pageNumber) * pageSize).Take(pageSize).OrderBy(x => x.Position).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Code.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "Position" ? x.Position : null);
                }
                else
                {
                    Query.OrderBy(x => column == "Position" ? x.Position : null);
                }
            }
        }
    }
}
