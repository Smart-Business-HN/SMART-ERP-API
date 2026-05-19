namespace SMART.ERP.Application.DTOs.Provider
{
    public class ProviderSummaryDto
    {
        public ProviderDto Provider { get; set; } = null!;
        public decimal TotalPurchased { get; set; }
        public decimal OutstandingBalance { get; set; }
        public decimal TotalPayments { get; set; }
        public int BillCount { get; set; }
        public decimal AveragePurchase { get; set; }
        public List<TopPurchasedProductFromProviderDto> TopProducts { get; set; } = new();
        public List<PurchaseDateCountDto> PurchaseActivity { get; set; } = new();
    }
}
