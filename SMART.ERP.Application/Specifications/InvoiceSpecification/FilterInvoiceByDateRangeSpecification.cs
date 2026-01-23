using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InvoiceSpecification
{
    public class FilterInvoiceByDateRangeSpecification : Specification<Invoice>
    {
        public FilterInvoiceByDateRangeSpecification(DateTime startDate, DateTime endDate)
        {
            Query.Where(x => x.CreationDate >= startDate && x.CreationDate <= endDate)
                .AsNoTracking();
        }
    }
}
