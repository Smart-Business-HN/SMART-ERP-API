using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class InterestLevel
    {
        public int Id { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
