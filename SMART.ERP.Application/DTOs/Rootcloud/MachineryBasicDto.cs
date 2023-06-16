namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class MachineryBasicDto
    {
        public string DeviceName { get; set; } = null!;
        public string SerialNum { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Province { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string Customer { get; set; } = null!;
        public bool ActiveMaintenance { get; set; }
        public string? Status { get; set; }
    }
}
