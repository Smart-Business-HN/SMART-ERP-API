using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.JournalEntrySpecification
{
    public class FilterJournalEntryByIdSpecification : Specification<JournalEntry>
    {
        public FilterJournalEntryByIdSpecification(int id, bool asTracking = false)
        {
            Query.Include(x => x.FiscalPeriod)
                 .Include(x => x.Lines!).ThenInclude(l => l.LedgerAccount)
                 .Include(x => x.Lines!).ThenInclude(l => l.Project)
                 .Include(x => x.Lines!).ThenInclude(l => l.Customer)
                 .Include(x => x.Lines!).ThenInclude(l => l.Provider)
                 .Where(x => x.Id == id);

            if (!asTracking)
                Query.AsNoTracking();
        }
    }
}
