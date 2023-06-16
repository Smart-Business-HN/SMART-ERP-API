namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class InternalMaintenanceDto
    {
        public int Quantity { get; set; }
        public bool InternalMaintenance { get; set; }
        public List<MachineryMissingMaintenanceDto> Machineries { get; set; } = new List<MachineryMissingMaintenanceDto>();
    }
}
