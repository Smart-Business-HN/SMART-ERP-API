using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Specifications.JournalEntrySpecification
{
    public class FilterDraftJournalEntriesByYearSpecification : Specification<JournalEntry>
    {
        public FilterDraftJournalEntriesByYearSpecification(DateTime start, DateTime end)
        {
            Query.Where(x => x.Status == JournalEntryStatus.Draft && x.EntryDate >= start && x.EntryDate <= end).AsNoTracking();
        }
    }
}
