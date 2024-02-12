namespace SMART.ERP.Domain.Entities
{
    public class PurchaseBillPayment
    {
        public int Id { get; init; }
        public int PurchaseBillId { get; set; }
        public PurchaseBill? PurchaseBill { get; set; }
        public int TypeOfPaymentMethodId { get; set; }
        public TypeOfPaymentMethod? TypeOfPaymentMethod { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int? InternalBankAccountId { get; set; }
        public InternalBankAccount? InternalBankAccount { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public string? Attachment { get; set; }
    }
}
