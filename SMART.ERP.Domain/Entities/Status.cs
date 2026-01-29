using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class Status
    {
        public int Id { get; init; }
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = null!;
        public int TypeStatusId { get; set; }
        public TypeStatus? TypeStatus { get; set; }
        public bool IsActive { get; set; }
    }
}
