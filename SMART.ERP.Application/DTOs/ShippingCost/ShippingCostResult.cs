namespace SMART.ERP.Application.DTOs.ShippingCost
{
    public class ShippingCostResult
    {
        public decimal MinCost { get; set; }
        public decimal MaxCost { get; set; }
        public decimal DefaultCost { get; set; }
        public string CostType { get; set; } = null!;
        public int? SourceWarehouseId { get; set; }
        public string? SourceWarehouseName { get; set; }
        public int? SourceProviderId { get; set; }
        public string? SourceProviderName { get; set; }
        public bool IsFromVirtualStock { get; set; }
    }
}
