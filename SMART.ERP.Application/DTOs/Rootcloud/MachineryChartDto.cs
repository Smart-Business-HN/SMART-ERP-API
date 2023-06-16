namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class MachineryChartDto
    {
        public List<InternalMaintenanceDto> InternalMaintenances { get; set; } = new List<InternalMaintenanceDto>();
        public List<MachineriesByFailureDto> MachineriesByFailure { get; set; } = new List<MachineriesByFailureDto>();
        public List<MachineryByProvinceDto> MachineryByProvince { get; set; } = new List<MachineryByProvinceDto>();
        public List<MachineriesByStatusDto> MachineriesByStatus { get; set; } = new List<MachineriesByStatusDto>();
        public MissingForNextMaintenanceDto? MissingForNextMaintenance { get; set; }
        public MachinerySystemRunningDto? MachinerySystemRunning { get; set; }
    }
}
