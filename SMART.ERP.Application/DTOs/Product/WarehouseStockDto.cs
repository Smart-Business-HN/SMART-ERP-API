namespace SMART.ERP.Application.DTOs.Product
{
    public class WarehouseStockDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = null!;
        public string WarehouseTypeName { get; set; } = null!;
        public bool IsVirtual { get; set; }
        public decimal Quantity { get; set; }
        public string? ProviderName { get; set; }
        public decimal? EstimatedShippingCost { get; set; }
        public string? CityName { get; set; }
    }
}
