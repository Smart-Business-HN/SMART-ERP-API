namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class MachineryMissingMaintenanceDto
    {
        public string SerialNum { get; set; } = null!;
        public string? Customer { get; set; }
        public decimal Hourmeter { get; set; }
        public string? Model { get; set; }
        public string? Region { get; set; }
        public decimal NextMaintenance { get; set; }
        public decimal MissingForNextMaintenance { get; set; }
        public decimal LateHours { get; set; }
        public string? Failure { get; set; }
        public DateTime? CreationDateFailure { get; set; }
        public string TimestampLocal { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public string Country { get; set; } = null!;
        public string Province { get; set; } = null!;
        public bool SystemRunning { get; set; }
    }
}
