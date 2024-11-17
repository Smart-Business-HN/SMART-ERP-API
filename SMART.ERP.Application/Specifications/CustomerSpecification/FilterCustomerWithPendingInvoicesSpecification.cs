using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CustomerSpecification
{
    public class FilterCustomerWithPendingInvoicesSpecification : Specification<Customer>
    {
        public FilterCustomerWithPendingInvoicesSpecification(string? parameter, int pageNumber,
            int pageSize, string? order, string? column)
        {
            Query.Include(x => x.PendingInvoices).Skip((pageNumber) * pageSize).Take(pageSize).Where(x => x.PendingInvoices!.Any(y => y.Outstanding > 0 && y.InvoicePaymentType!.Name == "Credito")).OrderByDescending(x => x.Id).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.FullName.Contains(parameter)); ;
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "FullName" ? x.FullName : null);
                }
                else
                {
                    Query.OrderBy(x => column == "FullName" ? x.FullName : null);
                }
            }
        }
    }
}
