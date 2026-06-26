namespace SMART.ERP.Application.DTOs.Report
{
    public class PurchaseSuggestionDto
    {
        public int ProductId { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string SubCategory { get; set; } = null!;
        public int ProviderId { get; set; }
        public string SupplierName { get; set; } = null!;
        public string UnitOfMeasurement { get; set; } = null!;
        public decimal UnitsSold { get; set; }
        public decimal AverageDailySales { get; set; }
        public int CurrentStock { get; set; }
        public int MinStock { get; set; }
        public decimal? DaysOfCoverage { get; set; }
        public int CoverageDays { get; set; }
        public int SuggestedQuantity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal EstimatedCost { get; set; }
    }
}
