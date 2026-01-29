using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class Banner
    {
        public int Id { get; init; }
        [Column(TypeName = "varchar(max)")]
        public string Url { get; set; } = null!;
        [Column(TypeName = "varchar(max)")]
        public string FileName { get; set; } = null!;
        public int CompanyId { get; set; }
        public virtual Company? Company { get; set; }
        public bool IsActive { get; set; }
    }
}
