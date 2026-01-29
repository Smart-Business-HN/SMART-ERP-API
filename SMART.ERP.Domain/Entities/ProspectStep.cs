using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class ProspectStep
    {
        public int Id { get; init; }
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
