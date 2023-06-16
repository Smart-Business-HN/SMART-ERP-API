namespace SMART.ERP.Domain.Entities
{
    public class OpportunityComment
    {
        public int Id { get; set; }
        public string Message { get; set; } = null!;
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public int OpportunityId { get; set; }
        public virtual Opportunity? Opportunity { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        public string? ModificatedBy { get; set; }
    }
}
