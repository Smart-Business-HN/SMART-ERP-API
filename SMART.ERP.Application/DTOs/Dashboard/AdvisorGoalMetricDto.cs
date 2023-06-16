namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class AdvisorGoalMetricDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public decimal Total { get; set; }
        public decimal SalesGoal { get; set; }
    }
}
