using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.RecurringInvoiceTemplateSpecification
{
    public class FilterRecurringInvoiceTemplateByIdSpecification : Specification<RecurringInvoiceTemplate>
    {
        public FilterRecurringInvoiceTemplateByIdSpecification(int id)
        {
            Query.Include(x => x.Customer!)
                .Include(x => x.BranchOffice!)
                .Include(x => x.User!)
                .Include(x => x.InvoicePaymentType!)
                .Include(x => x.Status!)
                .Include(x => x.Items!)
                .Where(x => x.Id == id)
                .AsNoTracking();
        }
    }
}
