using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class Country
    {
        public int Id { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = null!;
        [Column(TypeName = "varchar(10)")]
        public string Abbreviation { get; set; } = null!;
        [Column(TypeName = "varchar(20)")]
        public string PhoneNumberCode { get; set; } = null!;
        public bool IsActive { get; set; }
        public List<Region>? Regions { get; set; } = new List<Region>();
        public List<Department> Departments { get; set; } = new List<Department>();
    }
}
