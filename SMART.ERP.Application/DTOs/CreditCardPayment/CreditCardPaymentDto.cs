namespace SMART.ERP.Application.DTOs.CreditCardPayment
{
    public class CreditCardPaymentDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public int CreditCardInternalBankAccountId { get; set; }
        public string? CreditCardName { get; set; }
        public string? CreditCardLast4 { get; set; }
        public int SourceInternalBankAccountId { get; set; }
        public string? SourceBankName { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string? Reference { get; set; }
        public string? Notes { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
    }
}
