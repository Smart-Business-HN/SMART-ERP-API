using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class Company
    {
        public int Id { get; init; }
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = null!;
        [Column(TypeName = "varchar(50)")]
        public string Email { get; set; } = null!;
        [Column(TypeName = "varchar(50)")]
        public string PhoneNumber { get; set; } = null!;
        [Column(TypeName = "varchar(max)")]
        public string Address { get; set; } = null!;
        [Column(TypeName = "varchar(300)")]
        public string AboutUs { get; set; } = null!;
        [Column(TypeName = "varchar(150)")]
        public string? FacebookUrl { get; set; }
        [Column(TypeName = "varchar(150)")]
        public string? TwitterUrl { get; set; }
        [Column(TypeName = "varchar(150)")]
        public string? InstagramUrl { get; set; }
        [Column(TypeName = "varchar(150)")]
        public string? YoutubeUrl { get; set; }
        [Column(TypeName = "date")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        [Column(TypeName = "date")]
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModificatedBy { get; set; }
        public bool IsActive { get; set; }
        public int? CaiId { get; set; }
        public virtual Cai? Cai { get; set; }
        public List<BranchOffices>? BranchOffices { get; set; }
        public List<Opinion>? Opinions { get; set; }
        public List<Banner>? Banners { get; set; }
    }
}
