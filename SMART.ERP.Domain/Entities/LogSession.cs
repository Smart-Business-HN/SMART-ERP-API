using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class LogSession
    {
        public int Id { get; init; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime RegisterDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ExpirationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string IP { get; set; } = null!;
        [Column(TypeName = "varchar(150)")]
        public string DeviceInfo { get; set; } = null!;
        [Column(TypeName = "varchar(50)")]
        public string? Lat { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? Lng { get; set; }
        public bool IsActive { get; set; }
    }
}
