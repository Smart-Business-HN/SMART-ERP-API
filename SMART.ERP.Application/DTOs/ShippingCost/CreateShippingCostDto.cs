namespace SMART.ERP.Application.DTOs.ShippingCost
{
    public class CreateShippingCostDto
    {
        public int? SourceWarehouseId { get; set; }
        public int? SourceProviderId { get; set; }
        public int? SourceCityId { get; set; }
        public int? DestinationCityId { get; set; }
        public int? DestinationDepartmentId { get; set; }
        public int? ProductId { get; set; }
        public decimal MinCost { get; set; }
        public decimal MaxCost { get; set; }
        public decimal DefaultCost { get; set; }
        public string CostType { get; set; } = null!;
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public int Priority { get; set; }
    }
}
