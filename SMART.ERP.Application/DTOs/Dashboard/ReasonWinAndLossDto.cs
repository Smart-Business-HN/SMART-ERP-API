namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class ReasonWinAndLossDto
    {
        public string MetricName { get; set; } = null!;
        public string CurrentReasonName { get; set; } = null!;
        public string PreviousReasonName { get; set; } = null!;
        public decimal Current { get; set; }
        public decimal Previous { get; set; }
        public decimal Difference { get; set; }
        public decimal Percentage { get; set; }
    }
}
