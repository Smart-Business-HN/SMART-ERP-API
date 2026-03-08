using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.RecurringInvoiceTemplateSpecification
{
    public class FilterActiveTemplatesDueTodaySpecification : Specification<RecurringInvoiceTemplate>
    {
        public FilterActiveTemplatesDueTodaySpecification(DateTime today)
        {
            Query.Where(x => x.IsActive && x.NextGenerationDate != null && x.NextGenerationDate.Value.Date <= today.Date)
                .Include(x => x.Items!)
                .Include(x => x.Customer!)
                .AsNoTracking();
        }
    }
}
