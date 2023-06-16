namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class MachineryMaintenanceDto
    {
        public int Id { get; set; }
        public string Person { get; set; } = null!;
        public string? UrlDocument { get; set; }
        public string? Observation { get; set; }
        public decimal Hourmeter { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public int MachineryId { get; set; }
        public MachineryBasicDto? Machinery { get; set; }
    }
}
