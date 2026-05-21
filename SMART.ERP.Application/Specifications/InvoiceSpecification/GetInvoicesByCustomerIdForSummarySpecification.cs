using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InvoiceSpecification
{
    public class GetInvoicesByCustomerIdForSummarySpecification : Specification<Invoice>
    {
        public GetInvoicesByCustomerIdForSummarySpecification(Guid customerId)
        {
            Query
                .Where(x => x.CustomerId == customerId)
                .Include(x => x.ProductsSold!).ThenInclude(p => p.Product)
                .Include(x => x.BillPayments)
                .AsNoTracking();
        }
    }
}
