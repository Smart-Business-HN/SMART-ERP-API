using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Specifications.AccountingReportSpecification
{
    /// <summary>
    /// Partidas de asientos efectivos (Posted o ReversalEntry) dentro de un rango de fechas,
    /// con la cuenta y la cabecera incluidas. Base para todos los reportes contables.
    /// </summary>
    public class FilterPostedLinesByDateRangeSpecification : Specification<JournalEntryLine>
    {
        public FilterPostedLinesByDateRangeSpecification(DateTime fromDate, DateTime toDate, int? ledgerAccountId = null)
        {
            var from = fromDate.Date;
            var to = toDate.Date;
            Query.Include(x => x.LedgerAccount)
                 .Include(x => x.JournalEntry)
                 .Where(x => x.JournalEntry!.EntryDate >= from && x.JournalEntry.EntryDate <= to)
                 .Where(x => x.JournalEntry!.Status == JournalEntryStatus.Posted
                          || x.JournalEntry.Status == JournalEntryStatus.ReversalEntry)
                 .OrderBy(x => x.JournalEntry!.EntryDate).ThenBy(x => x.JournalEntry!.Id).ThenBy(x => x.LineNumber)
                 .AsNoTracking();

            if (ledgerAccountId.HasValue)
                Query.Where(x => x.LedgerAccountId == ledgerAccountId.Value);
        }
    }
}
