using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Specifications.JournalEntrySpecification
{
    public class FilterAndPaginationJournalEntrySpecification : Specification<JournalEntry>
    {
        public FilterAndPaginationJournalEntrySpecification(string? parameter, int pageNumber, int pageSize,
            DateTime? fromDate, DateTime? toDate, JournalEntryStatus? status)
        {
            Query.Include(x => x.FiscalPeriod)
                 .Include(x => x.Lines!).ThenInclude(l => l.LedgerAccount)
                 .OrderByDescending(x => x.EntryDate).ThenByDescending(x => x.Id)
                 .Skip(pageNumber * pageSize)
                 .Take(pageSize)
                 .AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => (x.EntryNumber != null && x.EntryNumber.Contains(parameter))
                    || x.Description.Contains(parameter)
                    || (x.Reference != null && x.Reference.Contains(parameter)));
            }

            if (fromDate.HasValue)
                Query.Where(x => x.EntryDate >= fromDate.Value);
            if (toDate.HasValue)
                Query.Where(x => x.EntryDate <= toDate.Value);
            if (status.HasValue)
                Query.Where(x => x.Status == status.Value);
        }
    }
}
