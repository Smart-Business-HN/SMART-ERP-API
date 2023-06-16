namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class LostOpportunityMetricDto
    {
        public string Month { get; set; } = null!;
        public decimal? Total { get; set; }
        public decimal GoalTotal { get; set; }
        public decimal WonTotal { get; set; }
        public int NumOpportunities { get; set; }
    }
}
