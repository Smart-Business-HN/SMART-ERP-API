using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.JournalEntryLineSpecification
{
    public class FilterJournalEntryLineByAccountSpecification : Specification<JournalEntryLine>
    {
        public FilterJournalEntryLineByAccountSpecification(int ledgerAccountId)
        {
            Query.Where(x => x.LedgerAccountId == ledgerAccountId).AsNoTracking();
        }
    }
}
