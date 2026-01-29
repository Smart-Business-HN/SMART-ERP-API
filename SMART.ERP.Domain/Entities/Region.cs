using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class Region
    {
        public int Id { get; init; }
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = null!;
        public int CountryId { get; set; }
        public virtual Country? Country { get; set; }
        public bool IsActive { get; set; }
        public List<Department>? Departments { get; set; } = new List<Department>();
    }
}
