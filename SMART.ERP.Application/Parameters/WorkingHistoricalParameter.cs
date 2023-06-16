namespace SMART.ERP.Application.Parameters
{
    public class WorkingHistoricalParameter
    {
        public string BaseInfoId { get; set; } = null!;
        public string DeviceModelId { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
