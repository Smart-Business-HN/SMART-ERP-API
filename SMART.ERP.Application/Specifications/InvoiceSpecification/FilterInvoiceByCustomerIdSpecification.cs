using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InvoiceSpecification
{
    public class FilterInvoiceByCustomerIdSpecification : Specification<Invoice>
    {
        public FilterInvoiceByCustomerIdSpecification(Guid customerId, string? parameter, int pageNumber, int pageSize)
        {
            Query
                .Include(x => x.Status)
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CreationDate)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.InvoiceNumber.Contains(parameter));
            }

            Query.Skip(pageNumber * pageSize).Take(pageSize);
        }
    }

    public class CountInvoiceByCustomerIdSpecification : Specification<Invoice>
    {
        public CountInvoiceByCustomerIdSpecification(Guid customerId, string? parameter)
        {
            Query
                .Where(x => x.CustomerId == customerId)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.InvoiceNumber.Contains(parameter));
            }
        }
    }
}
