using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class FinancingPlan
    {
        public int Id { get; init; }
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = null!;
        [Column(TypeName = "varchar(600)")]
        public string Description { get; set; } = null!;
        public int ProviderId { get; set; }
        public virtual Provider? Provider { get; set; }
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModificatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
