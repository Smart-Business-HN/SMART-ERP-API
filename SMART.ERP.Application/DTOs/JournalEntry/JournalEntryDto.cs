using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.DTOs.JournalEntry
{
    public class JournalEntryDto
    {
        public int Id { get; set; }
        public string? EntryNumber { get; set; }
        public DateTime EntryDate { get; set; }
        public int FiscalPeriodId { get; set; }
        public string? FiscalPeriodName { get; set; }
        public string Description { get; set; } = null!;
        public string? Reference { get; set; }
        public JournalEntryStatus Status { get; set; }
        public string StatusName { get; set; } = null!;
        public JournalEntrySource Source { get; set; }
        public string SourceName { get; set; } = null!;
        public string? SourceDocumentType { get; set; }
        public int? SourceDocumentId { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public DateTime? PostedDate { get; set; }
        public string? PostedBy { get; set; }
        public int? ReversesJournalEntryId { get; set; }
        public int? ReversedByJournalEntryId { get; set; }
        public DateTime CreationDate { get; set; }
        public string? CreatedBy { get; set; }
        public List<JournalEntryLineDto> Lines { get; set; } = new();
    }
}
