using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class BranchOffices
    {
        public int Id { get; init; }
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = null!;
        [Column(TypeName = "varchar(30)")]
        public string Email { get; set; } = null!;
        [Column(TypeName = "varchar(15)")]
        public string PhoneNumber { get; set; } = null!;
        [Column(TypeName = "varchar(150)")]
        public string Address { get; set; } = null!;
        [Column(TypeName = "real")]
        public float Lat { get; set; }
        [Column(TypeName = "real")]
        public float Lng { get; set; }
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
        public virtual Company? Company { get; set; }
        public int? CityId { get; set; }
        public virtual City? City { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? MapsId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModificatedBy { get; set; }
        public bool IsMainBranchOffice { get; set; }
        public List<Cai>? Cais { get; set; }
    }
}
