namespace SMART.ERP.Application.DTOs.JournalEntry
{
    public class JournalEntryLineDto
    {
        public int Id { get; set; }
        public int LedgerAccountId { get; set; }
        public string? LedgerAccountCode { get; set; }
        public string? LedgerAccountName { get; set; }
        public int LineNumber { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? Description { get; set; }
        public int? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int? ProviderId { get; set; }
        public string? ProviderName { get; set; }
    }
}
