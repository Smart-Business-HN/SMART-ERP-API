using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class ProductOffered
    {
        public int Id { get; init; }
        public int QuotationId { get; set; }
        public virtual Quotation? Quotation { get; set; }
        public int? ProductId { get; set; }
        public virtual Product? Product { get; set; }
        [MaxLength(50)]
        public string? ProductCode { get; set; }
        [MaxLength(2000)]
        public string ProductDescription { get; set; } = null!;
        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }
        [Precision(18, 2)]
        public decimal Quantity { get; set; }
        public int TaxId { get; set; }
        public virtual Tax? Tax { get; set; }
        [Precision(18, 2)]
        public decimal Taxes { get; set; }
        [Precision(18, 2)]
        public decimal TotalLine { get; set; }
    }
}
