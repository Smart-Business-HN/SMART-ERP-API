namespace SMART.ERP.Domain.Entities
{
    public class Invoice
    {
        public int Id { get; init; }
        public int CaiId { get; set; }
        public virtual Cai? Cai { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public int? QuotationOriginId { get; set; }
        public virtual Quotation? QuotationOrigin { get; set; }
        public Guid CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public int BranchOfficeId { get; set; }
        public virtual BranchOffices? BranchOffice { get; set; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public DateTime CreationDate { get; set; }
        public string? PurchaseOrderCode { get; set; }
        public string? SagCode { get; set; }
        public string? ExemptOrderNumber { get; set; }
        public string? ExemptedRegistrationCertificateNumber { get; set; }
        public decimal Exempt { get; set; }
        public decimal Exonerated { get; set; }
        public decimal TaxedAt15Percent { get; set; }
        public decimal TaxedAt18Percent { get; set; }
        public decimal Taxes15Percent { get; set; }
        public decimal Taxes18Percent { get; set; }
        public decimal Total { get; set; }
        public decimal Outstanding { get; set; }
        public string? Observations { get; set; }
        public string? TermsAndConditions { get; set; }
        public int StatusId { get; set; }
        public virtual Status? Status { get; set; }
        public int InvoicePaymentTypeId { get; set; }
        public virtual InvoicePaymentType? InvoicePaymentType { get; set; }
        public DateOnly? ExpectedPaymentDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime InsertedDate { get; set; }
        public string? ModificatedBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        public List<ProductSold>? ProductsSold { get; set; }
        public List<BillPayment>? BillPayments { get; set; }
    }
}
