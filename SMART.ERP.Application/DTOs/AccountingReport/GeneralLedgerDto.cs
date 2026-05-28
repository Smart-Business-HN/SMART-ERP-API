namespace SMART.ERP.Application.DTOs.AccountingReport
{
    /// <summary>Libro Mayor / Mayor general de una cuenta para un rango de fechas.</summary>
    public class GeneralLedgerDto
    {
        public int LedgerAccountId { get; set; }
        public string AccountCode { get; set; } = null!;
        public string AccountName { get; set; } = null!;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal ClosingBalance { get; set; }
        public List<GeneralLedgerLineDto> Movements { get; set; } = new();
    }

    public class GeneralLedgerLineDto
    {
        public DateTime EntryDate { get; set; }
        public string? EntryNumber { get; set; }
        public int JournalEntryId { get; set; }
        public string Description { get; set; } = null!;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal RunningBalance { get; set; }
    }
}
