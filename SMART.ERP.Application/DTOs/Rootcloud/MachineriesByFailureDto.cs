namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class MachineriesByFailureDto
    {
        public int Quantity { get; set; }
        public string Failure { get; set; } = null!;
        public List<MachineryMissingMaintenanceDto> Machineries { get; set; } = new List<MachineryMissingMaintenanceDto>();
    }
}
