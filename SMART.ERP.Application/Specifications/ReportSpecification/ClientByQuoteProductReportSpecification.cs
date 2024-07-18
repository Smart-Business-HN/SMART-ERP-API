using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ReportSpecification
{
    public class ClientByQuoteProductReportSpecification : Specification<Quotation>
    {
        public ClientByQuoteProductReportSpecification(int productId)
        {
            Query.Include(x => x.Customer).Include(x => x.ProductsOffered.Where(y => y.ProductId == productId))
                .Include(x => x.User).Where(x => x.ProductsOffered.Any(y => y.ProductId == productId));
        }
    }
}
