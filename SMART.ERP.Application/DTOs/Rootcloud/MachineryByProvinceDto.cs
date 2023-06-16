namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class MachineryByProvinceDto
    {
        public string Province { get; set; } = null!;
        public string Country { get; set; } = null!;
        public int Quantity { get; set; }
        public List<MachineryMissingMaintenanceDto> Machineries { get; set; } = new List<MachineryMissingMaintenanceDto>();
    }
}
