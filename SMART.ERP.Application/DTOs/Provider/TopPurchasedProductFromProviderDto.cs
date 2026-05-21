namespace SMART.ERP.Application.DTOs.Provider
{
    public class TopPurchasedProductFromProviderDto
    {
        public int? ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal QuantityPurchased { get; set; }
        public decimal Spent { get; set; }
    }
}
