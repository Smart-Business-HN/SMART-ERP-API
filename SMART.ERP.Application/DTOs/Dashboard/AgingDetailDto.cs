namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class AgingDetailDto
    {
        public string EntityId { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public decimal Outstanding { get; set; }
        public int DaysOverdue { get; set; }
        public string AgingBucket { get; set; } = string.Empty;
    }
}
