using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class Warehouse
    {
        public int Id { get; init; }
        [MaxLength(50)]
        public string Name { get; set; } = null!;
        [MaxLength(400)]
        public string? Address { get; set; }
        public Guid? UserId { get; set; }
        public virtual User? User { get; set; }
        public int? BranchOfficeId { get; set; }
        public virtual BranchOffices? BranchOffice { get; set; }
        public List<InventoryDistribution>? InventoryDistributions { get; set; }
        public bool IsGeneralWarehouse { get; set; }
        public int? CityId { get; set; }
        public virtual City? City { get; set; }
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModificatedBy { get; set; }
    }
}
