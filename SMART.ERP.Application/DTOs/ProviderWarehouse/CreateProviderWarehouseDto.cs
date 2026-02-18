namespace SMART.ERP.Application.DTOs.ProviderWarehouse
{
    public class CreateProviderWarehouseDto
    {
        public int ProviderId { get; set; }
        public int WarehouseId { get; set; }
        public bool IsActive { get; set; }
    }
}
