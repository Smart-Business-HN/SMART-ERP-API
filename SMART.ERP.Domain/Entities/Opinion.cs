using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class Opinion
    {
        public int Id { get; init; }
        [Column(TypeName = "varchar(50)")]
        public string Title { get; set; } = null!;
        [Column(TypeName = "varchar(max)")]
        public string? Image { get; set; }
        [Column(TypeName = "varchar(300)")]
        public string Observation { get; set; } = null!;
        [Column(TypeName = "varchar(50)")]
        public string Customer { get; set; } = null!;
        [Column(TypeName = "varchar(50)")]
        public string Charge { get; set; } = null!;
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
        public virtual Company? Company { get; set; }
    }
}
