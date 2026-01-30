using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class Invoice
    {
        public int Id { get; init; }
        public int CaiId { get; set; }
        public virtual Cai? Cai { get; set; }
        [MaxLength(19)]
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
        [MaxLength(50)]
        public string? PurchaseOrderCode { get; set; }
        [MaxLength(50)]
        public string? SagCode { get; set; }
        [MaxLength(50)]
        public string? ExemptOrderNumber { get; set; }
        [MaxLength(50)]
        public string? ExemptedRegistrationCertificateNumber { get; set; }
        [Precision(18, 2)]
        public decimal Exempt { get; set; }
        [Precision(18, 2)]
        public decimal Exonerated { get; set; }
        [Precision(18, 2)]
        public decimal TaxedAt15Percent { get; set; }
        [Precision(18, 2)]
        public decimal TaxedAt18Percent { get; set; }
        [Precision(18, 2)]
        public decimal Taxes15Percent { get; set; }
        [Precision(18, 2)]
        public decimal Taxes18Percent { get; set; }
        [Precision(18, 2)]
        public decimal Total { get; set; }
        [Precision(18, 2)]
        public decimal Outstanding { get; set; }
        [MaxLength(2000)]
        public string? Observations { get; set; }
        [MaxLength(2000)]
        public string? TermsAndConditions { get; set; }
        public int StatusId { get; set; }
        public virtual Status? Status { get; set; }
        public int InvoicePaymentTypeId { get; set; }
        public virtual InvoicePaymentType? InvoicePaymentType { get; set; }
        public DateOnly? ExpectedPaymentDate { get; set; }
        [MaxLength(50)]
        public string CreatedBy { get; set; } = null!;
        public DateTime InsertedDate { get; set; }
        [MaxLength(50)]
        public string? ModificatedBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        public List<ProductSold>? ProductsSold { get; set; }
        public List<BillPayment>? BillPayments { get; set; }
    }
}
