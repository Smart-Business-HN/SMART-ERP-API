
namespace SMART.ERP.Domain.Entities
{
    public class PurchaseBill
    {
        public int Id { get; init; }
        public string PurchaseBillCode { get; set; } = null!;
        public int ProviderId { get; set; }
        public Provider? Provider { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public DateTime InvoiceDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string Cai { get; set; } = null!;
        public int? PurchaseOrderOriginId { get; set; }
        public PurchaseOrder? PurchaseOrderOrigin { get; set; }
        public int StatusId { get; set; }
        public Status? Status { get; set; }
        public decimal Exempt {  get; set; }
        public decimal Exonerated { get; set; }
        public decimal TaxedAt15Percent { get; set; }
        public decimal TaxedAt18Percent { get; set; }
        public decimal Taxes15Percent {  get; set; }
        public decimal Taxes18Percent { get; set; }
        public decimal Total { get; set; }
        public decimal Outstanding { get; set; }
        public int? InventoryInputDestinationId { get; set; }
        public int PrefixId {  get; set; }
        public Prefix? Prefix { get; set; }
        public List<PurchaseBillPayment>? PurchaseBillPayments { get; set; }
        public int ExpenseAccountId { get; set; }
        public ExpenseAccount? ExpenseAccount { get; set; }
    }
}
