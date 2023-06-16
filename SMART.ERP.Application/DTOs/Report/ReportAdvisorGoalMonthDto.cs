namespace SMART.ERP.Application.DTOs.Report
{
    public class ReportAdvisorGoalMonthDto
    {
        public string Month { get; set; } = null!;
        public decimal Goal { get; set; }
        public decimal Total { get; set; }
    }
}
