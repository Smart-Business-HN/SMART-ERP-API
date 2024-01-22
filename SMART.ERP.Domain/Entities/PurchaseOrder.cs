namespace SMART.ERP.Domain.Entities
{
    public class PurchaseOrder
    {
        public int Id { get; init; }
        public int ProviderId { get; set; }
        public virtual Provider? Provider { get; set; }
        public string PurchaseOrderCode { get; set; } = null!;
        public int BranchOfficeId { get; set; }
        public virtual BranchOffices? BranchOffice {  get; set; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? Observations { get; set; }
        public string? TermsAndConditions { get; set;}
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public int StatusId { get; set; }
        public virtual Status? Status { get; set; }
        public int PrefixId { get; set; }
        public virtual Prefix? Prefix { get; set; }
        public int? PurchaseBillDestinationId { get; set; }
        public List<ProductToPurchase>? ProductsToPurchase { get; set; }
    }
}
