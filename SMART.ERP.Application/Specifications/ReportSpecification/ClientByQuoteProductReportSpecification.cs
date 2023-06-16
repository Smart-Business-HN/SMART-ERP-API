using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ReportSpecification
{
    public class ClientByQuoteProductReportSpecification : Specification<Opportunity>
    {
        public ClientByQuoteProductReportSpecification(int productId)
        {
            Query.Include(x => x.Customer).Include(x => x.OpportunityStep)
                .Include(x => x.User).Where(x => x.QuoteProducts.Any(y => y.ProductId == productId));
        }
    }
}
