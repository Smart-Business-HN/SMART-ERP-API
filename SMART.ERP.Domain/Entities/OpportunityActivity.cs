namespace SMART.ERP.Domain.Entities
{
    public class OpportunityActivity
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public DateTime InitDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TypeActivityId { get; set; }
        public virtual TypeActivity? TypeActivity { get; set; }
        public int StatusId { get; set; }
        public virtual Status? Status { get; set; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public int OpportunityId { get; set; }
        public virtual Opportunity? Opportunity { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        public string? ModificatedBy { get; set; }
        public string? GCEventId { get; set; }
    }
}
