using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.RecurringInvoiceTemplateSpecification
{
    public class QueryRecurringInvoiceTemplateSpecification : Specification<RecurringInvoiceTemplate>
    {
        public QueryRecurringInvoiceTemplateSpecification(string? parameter, int pageNumber, int pageSize)
        {
            Query.Include(x => x.Customer!)
                .Include(x => x.BranchOffice!)
                .Include(x => x.User!)
                .Include(x => x.InvoicePaymentType!)
                .Include(x => x.Status!)
                .Include(x => x.Items!)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .OrderByDescending(x => x.Id)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Customer!.FullName.Contains(parameter));
            }
        }
    }
}
