namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class WinAndLossMetricDto
    {
        public string MetricName { get; set; } = null!;
        public decimal Current { get; set; }
        public decimal Previous { get; set; }
        public decimal Difference { get; set; }
        public decimal Percentage { get; set; }
        public int Quantity { get; set; }
    }
}
