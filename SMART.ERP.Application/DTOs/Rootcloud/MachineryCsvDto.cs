namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class MachineryCsvDto
    {
        public string DeviceName { get; set; } = null!;
        public string SerialNum { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Province { get; set; } = null!;
        public string Customer { get; set; } = null!;
        public string ActiveMaintenance { get; set; } = null!;
        public string Interval { get; set; } = null!;
        public string InitialMaintenance { get; set; } = null!;
        public string Subcategory { get; set; } = null!;
        public string? Status { get; set; }
    }
}
