using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class Brand
    {
        public int Id { get; init; }
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = null!;
        [Column(TypeName = "varchar(200)")]
        public string? Description { get; set; }
        [Column(TypeName = "varchar(max)")]
        public string? Logo { get; set; }
        [Column(TypeName = "varchar(max)")]
        public string? Background { get; set; }
        public bool IsActive { get; set; }
    }
}
