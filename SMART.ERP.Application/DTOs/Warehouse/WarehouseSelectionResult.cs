namespace SMART.ERP.Application.DTOs.Warehouse
{
    public class WarehouseSelectionResult
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = null!;
        public bool IsVirtual { get; set; }
        public decimal AvailableQuantity { get; set; }
        public int? ProviderId { get; set; }
        public string? ProviderName { get; set; }
        public decimal? EstimatedShippingCost { get; set; }
        public string? CostType { get; set; }
    }
}
