using SMART.ERP.Application.DTOs.Opportunity;

namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class OpportunityStepMetricDto
    {
        public string Name { get; set; } = null!;
        public decimal Total { get; set; }
        public int NumOpportunities { get; set; }
        public int NumQuotes { get; set; }
        public List<OpportunityDto>? Opportunities { get; set; }
    }
}
