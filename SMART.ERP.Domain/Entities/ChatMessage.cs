using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class ChatMessage
    {
        public int Id { get; init; }

        public int ChatSessionId { get; set; }
        public virtual ChatSession? ChatSession { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string Content { get; set; } = null!;

        [Column(TypeName = "varchar(10)")]
        public string SenderType { get; set; } = null!;

        public Guid? SenderAdminUserId { get; set; }
        public virtual User? SenderAdminUser { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string SenderName { get; set; } = null!;

        [Column(TypeName = "datetime")]
        public DateTime SentAt { get; set; }

        public bool IsRead { get; set; }
    }
}
