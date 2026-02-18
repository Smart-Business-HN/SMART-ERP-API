namespace SMART.ERP.Application.DTOs.VirtualStock
{
    public class VirtualStockImportDetailDto
    {
        public int Id { get; set; }
        public int VirtualStockImportId { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string ProductCode { get; set; } = null!;
        public decimal Quantity { get; set; }
        public decimal? CostPrice { get; set; }
        public bool WasSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
