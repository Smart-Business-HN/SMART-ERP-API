namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class OpportunityDepartmentMetricDto
    {
        public string Department { get; set; } = null!;
        public int NumOpportunities { get; set; }
        public decimal Total { get; set; }
    }
}
