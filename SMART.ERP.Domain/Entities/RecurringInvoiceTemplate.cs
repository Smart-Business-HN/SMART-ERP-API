using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class RecurringInvoiceTemplate
    {
        public int Id { get; init; }
        public Guid CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public int BranchOfficeId { get; set; }
        public virtual BranchOffices? BranchOffice { get; set; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public int InvoicePaymentTypeId { get; set; }
        public virtual InvoicePaymentType? InvoicePaymentType { get; set; }
        public int DayOfMonth { get; set; }
        [MaxLength(2000)]
        public string? Observations { get; set; }
        [MaxLength(2000)]
        public string? TermsAndConditions { get; set; }
        public int StatusId { get; set; }
        public virtual Status? Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(50)]
        public string CreatedBy { get; set; } = null!;
        public DateTime? LastGeneratedDate { get; set; }
        public DateTime? NextGenerationDate { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        public List<RecurringInvoiceTemplateItem>? Items { get; set; }
    }
}
