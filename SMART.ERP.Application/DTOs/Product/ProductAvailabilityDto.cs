namespace SMART.ERP.Application.DTOs.Product
{
    public class ProductAvailabilityDto
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public decimal TotalPhysicalStock { get; set; }
        public decimal TotalVirtualStock { get; set; }
        public decimal TotalAvailableStock { get; set; }
        public List<WarehouseStockDto> StockByWarehouse { get; set; } = new();
    }
}
