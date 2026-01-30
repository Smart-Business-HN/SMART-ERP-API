using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class PurchaseBill
    {
        public int Id { get; init; }
        [MaxLength(50)]
        public string PurchaseBillCode { get; set; } = null!;
        public int ProviderId { get; set; }
        public virtual Provider? Provider { get; set; }
        [MaxLength(19)]
        public string InvoiceNumber { get; set; } = null!;
        public DateTime InvoiceDate { get; set; }
        public DateTime CreationDate { get; set; }
        [MaxLength(37)]
        public string Cai { get; set; } = null!;
        public int? PurchaseOrderOriginId { get; set; }
        public virtual PurchaseOrder? PurchaseOrderOrigin { get; set; }
        public int StatusId { get; set; }
        public virtual Status? Status { get; set; }
        [Precision(18, 2)]
        public decimal Exempt {  get; set; }
        [Precision(18, 2)]
        public decimal Exonerated { get; set; }
        [Precision(18, 2)]
        public decimal TaxedAt15Percent { get; set; }
        [Precision(18, 2)]
        public decimal TaxedAt18Percent { get; set; }
        [Precision(18, 2)]
        public decimal Taxes15Percent {  get; set; }
        [Precision(18, 2)]
        public decimal Taxes18Percent { get; set; }
        [Precision(18, 2)]
        public decimal Total { get; set; }
        [Precision(18, 2)]
        public decimal Outstanding { get; set; }
        public int? InventoryInputDestinationId { get; set; }
        public int PrefixId {  get; set; }
        public virtual Prefix? Prefix { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        public virtual List<PurchaseBillPayment>? PurchaseBillPayments { get; set; }
        public int ExpenseAccountId { get; set; }
        public virtual ExpenseAccount? ExpenseAccount { get; set; }
    }
}
