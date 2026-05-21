namespace SMART.ERP.Application.DTOs.Customer
{
    public class TopPurchasedProductDto
    {
        public int? ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }
}
