namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class WorkingHistoricalRequestDto
    {
        public long BaseInfoId { get; set; }
        public long DeviceModelId { get; set; }
        public string StartTime { get; set; } = null!;
        public string EndTime { get; set; } = null!;
        public string TenantId { get; set; } = null!;
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public List<string> Dimentions { get; set; } = new List<string>();
    }
}
