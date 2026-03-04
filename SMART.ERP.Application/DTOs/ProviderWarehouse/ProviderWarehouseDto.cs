namespace SMART.ERP.Application.DTOs.ProviderWarehouse
{
    public class ProviderWarehouseDto
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public string ProviderName { get; set; } = null!;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
