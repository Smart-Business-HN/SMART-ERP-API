namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class MissingForNextMaintenanceDto
    {
        public int TotalMachineries { get; set; }
        public int TotalNotMissing { get; set; }
        public int TotalMissing { get; set; }
        public List<MachineryMissingMaintenanceDto> Machineries { get; set; } = new List<MachineryMissingMaintenanceDto>();
    }
}
