using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class Quotation
    {
        public int Id { get; init; }
        public Guid CustomerId { get; set; }
        [MaxLength(50)]
        public string? QuotationCode { get; set; }
        public virtual Customer? Customer { get; set; }
        public int BranchOfficeId { get; set; }
        public virtual BranchOffices? BranchOffice { get; set; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DueDate { get; set; }
        [MaxLength(2000)]
        public string? Observations { get; set; }
        [MaxLength(2000)]
        public string? TermsAndConditions { get; set; }
        public List<ProductOffered>? ProductsOffered { get; set; }
        [Precision(18, 2)]
        public decimal SubTotal { get; set; }
        [Precision(18, 2)]
        public decimal Total { get; set; }
        public int StatusId { get; set; }
        public virtual Status? Status { get; set; }
        public int PrefixId { get; set; }
        public virtual Prefix? Prefix { get; set; }
        [Precision(18, 2)]
        public decimal? Profitability { get; set; }
        public int? InvoiceDestinationId { get; set; }
        public virtual Invoice? InvoiceDestination { get; set; }
        [MaxLength(50)]
        public string CreatedBy { get; set; } = null!;
        public DateTime InsertedDate { get; set; }
        [MaxLength(50)]
        public string? ModificatedBy { get; set; }
        public DateTime? ModificationDate { get; set; }
    }
}
