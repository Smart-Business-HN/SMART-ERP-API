
using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CustomerSpecification
{
    public class IncludePendingInvoicesByCustomerIdSpecification : Specification<Customer>
    {
        public IncludePendingInvoicesByCustomerIdSpecification(Guid customerId)
        {
            Query.Include(x => x.PendingInvoices!.Where(y => y.Outstanding > 0 && y.InvoicePaymentType!.Name == "Credito")).Where(x => x.Id == customerId).AsNoTracking();
        }
    }
}
