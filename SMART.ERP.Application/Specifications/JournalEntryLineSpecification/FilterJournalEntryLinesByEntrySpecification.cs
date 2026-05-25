using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.JournalEntryLineSpecification
{
    public class FilterJournalEntryLinesByEntrySpecification : Specification<JournalEntryLine>
    {
        public FilterJournalEntryLinesByEntrySpecification(int journalEntryId)
        {
            Query.Where(x => x.JournalEntryId == journalEntryId);
        }
    }
}
