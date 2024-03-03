using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InvoiceSpecification
{
    public class FilterInvoiceByPendingValuesSpecification : Specification<Invoice>
    {
        public FilterInvoiceByPendingValuesSpecification()
        {
            Query.Where(x=>x.Outstanding > 0);
        }
    }
}
