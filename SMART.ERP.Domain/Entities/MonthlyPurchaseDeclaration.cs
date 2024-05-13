namespace SMART.ERP.Domain.Entities
{
    public class MonthlyPurchaseDeclaration
    {
        public int Id { get; init; }
        public string Period { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public int StatusId { get; set; }
        public virtual Status? Status { get; set; }
        public int TotalPurchaseBills { get; set; }
        public decimal Exempt { get; set; }
        public decimal Exonerated { get; set; }
        public decimal TaxedAt15Percent { get; set; }
        public decimal TaxedAt18Percent { get; set; }
        public decimal Taxes15Percent { get; set; }
        public decimal Taxes18Percent { get; set; }
        public decimal TotalTaxes { get; set; }
        public decimal Total { get; set; }
        public List<DeclaratedPurchaseBill>? DeclaratedPurchaseBills { get; set; }
    }
}
