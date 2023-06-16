using SMART.ERP.Application.DTOs.Opportunity;

namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class AdvisorDashboardDto
    {
        public decimal MonthlyGoal { get; set; }
        public decimal MonthlyGoalPercentage { get; set; }
        public decimal AnnualGoal { get; set; }
        public decimal AnnualGoalPercentage { get; set; }
        public List<OpportunityActivityDto> OpportunityActivities { get; set; } = new List<OpportunityActivityDto>();
    }
}
