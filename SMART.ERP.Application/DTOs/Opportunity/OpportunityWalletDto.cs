namespace SMART.ERP.Application.DTOs.Opportunity
{
    public class OpportunityWalletDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public int QtyItems { get; set; }
        public int? ProbabilityPercentage { get; set; }
        public int OpportunityStepId { get; set; }
        public OpportunityStepDto? OpportunityStep { get; set; }
        public DateTime CreationDate { get; set; }
        public List<QuoteProductDto>? QuoteProducts { get; set; }
        public bool IsActive { get; set; }
    }
}
