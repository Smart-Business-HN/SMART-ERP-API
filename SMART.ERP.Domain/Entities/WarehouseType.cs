using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class WarehouseType
    {
        public int Id { get; init; }
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = null!;
        [Column(TypeName = "varchar(200)")]
        public string? Description { get; set; }
        public bool IsVirtual { get; set; }
        public bool IsActive { get; set; }
    }
}
