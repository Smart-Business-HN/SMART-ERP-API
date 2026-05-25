using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.JournalEntrySpecification
{
    /// <summary>
    /// Asientos contabilizados de un año cuyo número inicia con "{year}-", ordenados de mayor a menor,
    /// para derivar el siguiente correlativo.
    /// </summary>
    public class FilterJournalEntryByYearPrefixSpecification : Specification<JournalEntry>
    {
        public FilterJournalEntryByYearPrefixSpecification(int year)
        {
            var prefix = $"{year}-";
            Query.Where(x => x.EntryNumber != null && x.EntryNumber.StartsWith(prefix))
                 .OrderByDescending(x => x.EntryNumber)
                 .AsNoTracking();
        }
    }
}
