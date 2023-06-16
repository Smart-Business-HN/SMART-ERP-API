namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class MachinerySystemRunningDto
    {
        public int TotalMachineries { get; set; }
        public int SystemRunning { get; set; }
        public int SystemNotRunning { get; set; }
        public List<MachineryMissingMaintenanceDto> Machineries { get; set; } = new List<MachineryMissingMaintenanceDto>();
    }
}
