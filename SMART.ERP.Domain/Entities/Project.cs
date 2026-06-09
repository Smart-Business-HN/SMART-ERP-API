using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class Project
    {
        public int Id { get; init; }
        [MaxLength(150)]
        public string Name { get; set; } = null!;
        [MaxLength(50)]
        public string ProjectCode { get; set; } = null!;
        [MaxLength(2000)]
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Precision(18, 2)]
        public decimal ExecutionBudget { get; set; }
        public Guid CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public int StatusId { get; set; }
        public virtual Status? Status { get; set; }
        public int PrefixId { get; set; }
        public virtual Prefix? Prefix { get; set; }
        [MaxLength(50)]
        public string CreatedBy { get; set; } = null!;
        public DateTime InsertedDate { get; set; }
        [MaxLength(50)]
        public string? ModificatedBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        public virtual List<PurchaseBill>? PurchaseBills { get; set; }
        public virtual List<NonBillableExpense>? NonBillableExpenses { get; set; }
        public virtual List<Invoice>? Invoices { get; set; }
        public virtual List<Quotation>? Quotations { get; set; }
        public virtual List<ProjectAttachment>? ProjectAttachments { get; set; }
        public virtual List<InventoryExit>? InventoryExits { get; set; }
        public virtual List<InventoryEntry>? InventoryEntries { get; set; }
    }
}
