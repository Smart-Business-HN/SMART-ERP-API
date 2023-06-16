namespace SMART.ERP.Application.DTOs.Opportunity
{
    public class OpportunityStepDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Total { get; set; }
        public bool IsActive { get; set; }
    }
}
