namespace SMART.ERP.Application.DTOs.Product
{
    public class ProductPriceWithShippingDto
    {
        public decimal BasePrice { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsFromVirtualStock { get; set; }
        public int? SourceWarehouseId { get; set; }
        public string? SourceWarehouseName { get; set; }
        public string? ProviderName { get; set; }
        public string? EstimatedDeliveryDays { get; set; }
    }
}
