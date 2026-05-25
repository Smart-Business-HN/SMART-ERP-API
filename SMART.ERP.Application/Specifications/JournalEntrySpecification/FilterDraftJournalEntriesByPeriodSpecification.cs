using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Specifications.JournalEntrySpecification
{
    public class FilterDraftJournalEntriesByPeriodSpecification : Specification<JournalEntry>
    {
        public FilterDraftJournalEntriesByPeriodSpecification(int fiscalPeriodId)
        {
            Query.Where(x => x.FiscalPeriodId == fiscalPeriodId && x.Status == JournalEntryStatus.Draft).AsNoTracking();
        }
    }
}
