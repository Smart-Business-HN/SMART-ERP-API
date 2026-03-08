using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class RecurringInvoiceTemplateItem
    {
        public int Id { get; init; }
        public int RecurringInvoiceTemplateId { get; set; }
        public virtual RecurringInvoiceTemplate? RecurringInvoiceTemplate { get; set; }
        public int? ProductId { get; set; }
        public virtual Product? Product { get; set; }
        [MaxLength(20)]
        public string? ProductCode { get; set; }
        [MaxLength(500)]
        public string ProductDescription { get; set; } = null!;
        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }
        [Precision(18, 2)]
        public decimal Quantity { get; set; }
        public int TaxId { get; set; }
        public virtual Tax? Tax { get; set; }
    }
}
