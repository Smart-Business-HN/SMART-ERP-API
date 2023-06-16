namespace SMART.ERP.Domain.Entities
{
    public class OpportunityDocument
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Url { get; set; } = null!;
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public int DocumentTypeId { get; set; }
        public virtual DocumentType? DocumentType { get; set; }
        public int OpportunityId { get; set; }
        public virtual Opportunity? Opportunity { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        public string? ModificatedBy { get; set; }
    }
}
