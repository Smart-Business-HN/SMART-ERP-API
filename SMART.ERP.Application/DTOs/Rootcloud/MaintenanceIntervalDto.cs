namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class MaintenanceIntervalDto
    {
        public List<int> Categories { get; set; } = null!;
        public decimal Initial { get; set; }
        public decimal Interval { get; set; }
    }
}
