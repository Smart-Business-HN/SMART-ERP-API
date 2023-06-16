using SMART.ERP.Application.DTOs.Opportunity;

namespace SMART.ERP.Application.DTOs.Report
{
    public class ReportAdvisorActivitiesDto
    {
        public string FullName { get; set; } = null!;
        public List<OpportunityActivityDto>? Activities { get; set; }
        public int Finished { get; set; }
        public int Pending { get; set; }
        public int Total { get; set; }
        public decimal CompletionPercentage { get; set; }
    }
}
