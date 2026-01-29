using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class MonthlyPurchaseDeclaration
    {
        public int Id { get; init; }
        [MaxLength(6)]
        public string Period { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        [MaxLength(50)]
        public string CreatedBy { get; set; } = null!;
        public int StatusId { get; set; }
        public virtual Status? Status { get; set; }
        public int TotalPurchaseBills { get; set; }
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
        public decimal TotalTaxes { get; set; }
        [Precision(18, 2)]
        public decimal Total { get; set; }
        public List<DeclaratedPurchaseBill>? DeclaratedPurchaseBills { get; set; }
    }
}
