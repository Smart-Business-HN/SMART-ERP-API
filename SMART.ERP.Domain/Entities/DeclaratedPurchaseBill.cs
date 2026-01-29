using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class DeclaratedPurchaseBill
    {
        public int Id { get; init; }
        public int MonthlyPurchaseDeclarationId { get; set; }
        public virtual MonthlyPurchaseDeclaration? MonthlyPurchaseDeclaration { get; set; }
        public int PurchaseBillId { get; set; }
        public virtual PurchaseBill? PurchaseBill { get; set; }
        [MaxLength(14)]
        public string ProviderRTN { get; set; } = null!;
        [MaxLength(250)]
        public string ProviderName { get; set; } = null!;
        [MaxLength(10)]
        public string BillDate { get; set; } = null!;
        [MaxLength(37)]
        public string Cai { get; set; } = null!;
        [MaxLength(3)]
        public string Establishment { get; set; } = null!;
        [MaxLength(3)]
        public string EmissionPoint { get; set; } = null!;
        [MaxLength(2)]
        public string KindOfDocument { get; set; } = null!;
        [MaxLength(8)]
        public string Correlative { get; set; } = null!;
        [MaxLength(2)]
        public string PurchaseWithOce { get; set; } = null!;
        [MaxLength(50)]
        public string? ResolutionNumber { get; set; }
        [MaxLength(50)]
        public string? ResolutionDate { get; set; }
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
    }
}
