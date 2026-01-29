using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.DTOs.User;

namespace SMART.ERP.Application.DTOs.Opportunity
{
    public class OpportunityDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public Guid CustomerId { get; set; }
        public BasicInfoCustomerDto Customer { get; set; } = null!;
        public int Position { get; set; }
        public int QtyItems { get; set; }
        public decimal? Budget { get; set; }
        public bool? ApplyOnCredit { get; set; }
        public decimal Total { get; set; } = 0;
        public string? RecommendedBy { get; set; }
        public string? OpportunityType { get; set; }
        public DateTime? ExpectedClosingDate { get; set; }
        public DateTime? ClosingDate { get; set; }
        public int? ProbabilityPercentage { get; set; }
        public string? Description { get; set; }
        public int InterestLevelId { get; set; }
        public InterestLevelDto? InterestLevel { get; set; }
        public int OpportunityStepId { get; set; }
        public OpportunityStepDto? OpportunityStep { get; set; }
        public int? TypeOriginId { get; set; }
        public TypeOriginDto? TypeOrigin { get; set; }
        public int? LossReasonId { get; set; }
        public LossReasonDto? LossReason { get; set; }
        public int? WinReasonId { get; set; }
        public WinReasonDto? WinReason { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid UserId { get; set; }
        public UserDto? User { get; set; }
        public List<QuoteProductDto>? QuoteProducts { get; set; }
        public List<OpportunityActivityDto>? OpportunityActivities { get; set; }
        public List<OpportunityCommentDto>? OpportunityComments { get; set; }
        public List<OpportunityDocumentDto>? OpportunityDocuments { get; set; }
        public bool IsActive { get; set; }
    }
}
