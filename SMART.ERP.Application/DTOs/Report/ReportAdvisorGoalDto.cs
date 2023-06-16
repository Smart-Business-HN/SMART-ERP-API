namespace SMART.ERP.Application.DTOs.Report
{
    public class ReportAdvisorGoalDto
    {
        public string FullName { get; set; } = null!;
        public List<ReportAdvisorGoalMonthDto> Months { get; set; } = new List<ReportAdvisorGoalMonthDto>();
    }
}
