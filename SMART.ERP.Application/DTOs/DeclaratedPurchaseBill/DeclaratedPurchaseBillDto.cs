using SMART.ERP.Application.DTOs.MonthlyPurchaseDeclaration;
using SMART.ERP.Application.DTOs.PurchaseBill;

namespace SMART.ERP.Application.DTOs.DeclaratedPurchaseBill
{
    public class DeclaratedPurchaseBillDto
    {
        public int Id { get; init; }
        public int MonthlyPurchaseDeclarationId { get; set; }
        public virtual MonthlyPurchaseDeclarationDto? MonthlyPurchaseDeclaration { get; set; }
        public int PurchaseBillId { get; set; }
        public virtual PurchaseBillDto? PurchaseBill { get; set; }
        public string ProviderRTN { get; set; } = null!;
        public string ProviderName { get; set; } = null!;
        public string BillDate { get; set; } = null!;
        public string Cai { get; set; } = null!;
        public string Establishment { get; set; } = null!;
        public string EmissionPoint { get; set; } = null!;
        public string KindOfDocument { get; set; } = null!;
        public string Correlative { get; set; } = null!;
        public string PurchaseWithOce { get; set; } = null!;
        public string? ResolutionNumber { get; set; }
        public string? ResolutionDate { get; set; }
        public decimal Exempt { get; set; }
        public decimal Exonerated { get; set; }
        public decimal TaxedAt15Percent { get; set; }
        public decimal TaxedAt18Percent { get; set; }
        public decimal Taxes15Percent { get; set; }
        public decimal Taxes18Percent { get; set; }
    }
}
