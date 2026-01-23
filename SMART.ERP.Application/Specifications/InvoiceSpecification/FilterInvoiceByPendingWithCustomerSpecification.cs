using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InvoiceSpecification
{
    public class FilterInvoiceByPendingWithCustomerSpecification : Specification<Invoice>
    {
        public FilterInvoiceByPendingWithCustomerSpecification()
        {
            Query.Include(x => x.Customer)
                .Where(x => x.Outstanding > 0)
                .AsNoTracking();
        }
    }
}
