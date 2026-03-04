namespace SMART.ERP.Application.DTOs.VirtualStock
{
    public class VirtualStockImportDto
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public string ProviderName { get; set; } = null!;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public DateTime ImportDate { get; set; }
        public int TotalProducts { get; set; }
        public int SuccessfulImports { get; set; }
        public int FailedImports { get; set; }
        public string? ErrorLog { get; set; }
        public string ImportedBy { get; set; } = null!;
        public List<VirtualStockImportDetailDto>? ImportDetails { get; set; }
    }
}
