namespace SMART.ERP.Application.DTOs.Opportunity
{
    public class ActiveOpportunityDto
    {
        public bool IsActive { get; set; }
        public OpportunityDto? Opportunity { get; set; }
    }
}
