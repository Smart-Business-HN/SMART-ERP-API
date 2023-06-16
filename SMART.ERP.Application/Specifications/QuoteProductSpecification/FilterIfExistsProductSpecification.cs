using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.QuoteProductSpecification
{
    public class FilterIfExistsProductSpecification : Specification<QuoteProduct>
    {
        public FilterIfExistsProductSpecification(int productId, int opportunityId)
        {
            Query.Where(x => x.ProductId == productId && x.OpportunityId == opportunityId && x.IsActive).AsNoTracking();
        }
    }
}
