using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class OpportunityActivity
    {
        public int Id { get; set; }
        [Column(TypeName = "varchar(300)")]
        public string Description { get; set; } = null!;
        [Column(TypeName = "datetime2(2)")]
        public DateTime InitDate { get; set; }
        [Column(TypeName = "datetime2(2)")]
        public DateTime EndDate { get; set; }
        public int TypeActivityId { get; set; }
        public virtual TypeActivity? TypeActivity { get; set; }
        public int StatusId { get; set; }
        public virtual Status? Status { get; set; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public int OpportunityId { get; set; }
        public virtual Opportunity? Opportunity { get; set; }
        [Column(TypeName = "datetime2(2)")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        [Column(TypeName = "datetime2(2)")]
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModificatedBy { get; set; }
        [Column(TypeName = "varchar(300)")]
        public string? GCEventId { get; set; }
    }
}
