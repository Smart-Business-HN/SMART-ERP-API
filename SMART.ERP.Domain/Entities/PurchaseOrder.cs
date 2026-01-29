using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class PurchaseOrder
    {
        public int Id { get; init; }
        public int ProviderId { get; set; }
        public virtual Provider? Provider { get; set; }
        [MaxLength(50)]
        public string PurchaseOrderCode { get; set; } = null!;
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
        [Precision(18, 2)]
        public decimal Subtotal { get; set; }
        [Precision(18, 2)]
        public decimal Total { get; set; }
        public int StatusId { get; set; }
        public virtual Status? Status { get; set; }
        public int PrefixId { get; set; }
        public virtual Prefix? Prefix { get; set; }
        public int? PurchaseBillDestinationId { get; set; }
        public virtual PurchaseBill? PurchaseBillDestination { get; set; }
        public int? InventoryInputDestinationId { get; set; }
        public virtual InventoryInput? InventoryInputDestination { get; set; }
        public List<ProductToPurchase>? ProductsToPurchase { get; set; }
    }
}
