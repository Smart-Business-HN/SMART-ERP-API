using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.RecurringInvoiceTemplateSpecification
{
    public class FilterRecurringInvoiceLogsByTemplateIdSpecification : Specification<RecurringInvoiceLog>
    {
        public FilterRecurringInvoiceLogsByTemplateIdSpecification(int templateId, int pageNumber, int pageSize)
        {
            Query.Where(x => x.RecurringInvoiceTemplateId == templateId)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .OrderByDescending(x => x.ExecutionDate)
                .AsNoTracking();
        }
    }
}
