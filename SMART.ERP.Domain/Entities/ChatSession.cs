using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class ChatSession
    {
        public int Id { get; init; }

        [Column(TypeName = "varchar(36)")]
        public string SessionIdentifier { get; set; } = null!;

        [Column(TypeName = "varchar(100)")]
        public string VisitorName { get; set; } = null!;

        [Column(TypeName = "varchar(100)")]
        public string? VisitorEmail { get; set; }

        public bool IsAuthenticated { get; set; }

        public Guid? EcommerceUserId { get; set; }
        public virtual EcommerceUser? EcommerceUser { get; set; }

        public Guid? AssignedAdminUserId { get; set; }
        public virtual User? AssignedAdminUser { get; set; }

        public int Status { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ClosedAt { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string? LastMessagePreview { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LastMessageAt { get; set; }

        public int UnreadAdminCount { get; set; }

        public ICollection<ChatMessage>? Messages { get; set; }
    }
}
