using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class Subcategory
    {
        public int Id { get; init; }
        [MaxLength(400)]
        public string Slug { get; set; } = null!;
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = null!;
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModificatedBy { get; set; }
    }
}
