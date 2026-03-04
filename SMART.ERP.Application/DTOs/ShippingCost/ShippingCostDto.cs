namespace SMART.ERP.Application.DTOs.ShippingCost
{
    public class ShippingCostDto
    {
        public int Id { get; set; }
        public int? SourceWarehouseId { get; set; }
        public string? SourceWarehouseName { get; set; }
        public int? SourceProviderId { get; set; }
        public string? SourceProviderName { get; set; }
        public string? SourceCityName { get; set; }
        public int? DestinationCityId { get; set; }
        public string? DestinationCityName { get; set; }
        public int? DestinationDepartmentId { get; set; }
        public string? DestinationDepartmentName { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal MinCost { get; set; }
        public decimal MaxCost { get; set; }
        public decimal DefaultCost { get; set; }
        public string CostType { get; set; } = null!;
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public int Priority { get; set; }
    }
}
