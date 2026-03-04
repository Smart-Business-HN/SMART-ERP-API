using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; init; }

        [Column(TypeName = "varchar(128)")]
        public string TokenHash { get; set; } = null!;

        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string CreatedByIp { get; set; } = null!;

        public bool IsRevoked { get; set; }

        public DateTime? RevokedDate { get; set; }

        [Column(TypeName = "varchar(128)")]
        public string? ReplacedByTokenHash { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? RevokedReason { get; set; }
    }
}
