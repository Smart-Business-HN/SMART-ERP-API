namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class AlertSystemNotRunningDto
    {
        public string SerialNum { get; set; } = null!;
        public decimal Hourmeter { get; set; }
        public string TimestampLocal { get; set; } = null!;
    }
}
