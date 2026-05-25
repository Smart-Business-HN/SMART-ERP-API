using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Specifications.JournalEntrySpecification
{
    /// <summary>Misma cláusula de filtrado que la paginación, sin Skip/Take ni includes, para contar.</summary>
    public class FilterJournalEntryCountSpecification : Specification<JournalEntry>
    {
        public FilterJournalEntryCountSpecification(string? parameter, DateTime? fromDate, DateTime? toDate, JournalEntryStatus? status)
        {
            Query.AsNoTracking();

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
