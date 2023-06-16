namespace SMART.ERP.Domain.Entities
{
    public class Opportunity
    {
        public int Id { get; init; }
        public string Code { get; set; } = null!;
        public decimal Budget { get; set; }
        public DateTime? ExpectedClosingDate { get; set; }
        public DateTime? ClosingDate { get; set; }
        public int QtyItems { get; set; }
        public int ProbabilityPercentage { get; set; }
        public string? Description { get; set; }
        public bool ApplyOnCredit { get; set; }
        public string? RecommendedBy { get; set; }
        public string? OpportunityType { get; set; }
        public int? InterestLevelId { get; set; }
        public virtual InterestLevel? InterestLevel { get; set; }
        public decimal Total { get; set; }
        public int Position { get; set; }
        public int OpportunityStepId { get; set; }
        public virtual OpportunityStep? OpportunityStep { get; set; }
        public int? TypeOriginId { get; set; }
        public TypeOrigin? TypeOrigin { get; set; }
        public Guid CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsActive { get; set; }
        public int? LossReasonId { get; set; }
        public virtual LossReason? LossReason { get; set; }
        public int? WinReasonId { get; set; }
        public virtual WinReason? WinReason { get; set; }
        public List<QuoteProduct>? QuoteProducts { get; set; }
        public List<OpportunityActivity>? OpportunityActivities { get; set; }
        public List<OpportunityComment>? OpportunityComments { get; set; }
        public List<OpportunityDocument>? OpportunityDocuments { get; set; }
    }
}
