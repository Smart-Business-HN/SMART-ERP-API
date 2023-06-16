namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class MachineriesByStatusDto
    {
        public int Quantity { get; set; }
        public string Status { get; set; } = null!;
        public List<MachineryMissingMaintenanceDto> Machineries { get; set; } = new List<MachineryMissingMaintenanceDto>();
    }
}
