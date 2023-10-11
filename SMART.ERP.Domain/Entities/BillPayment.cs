
namespace SMART.ERP.Domain.Entities
{
    public class BillPayment
    {
        public int Id { get; init; }
        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
        public int TypeOfPaymentMethodId { get; set; }
        public TypeOfPaymentMethod? TypeOfPaymentMethod { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int? DestinationBankAccountId { get; set; }
        public InternalBankAccount? DestinationBankAccount { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public string? Attachment {  get; set; }
    }
}
