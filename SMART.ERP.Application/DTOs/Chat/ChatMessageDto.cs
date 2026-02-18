namespace SMART.ERP.Application.DTOs.Chat
{
    public class ChatMessageDto
    {
        public int Id { get; set; }
        public int ChatSessionId { get; set; }
        public string Content { get; set; } = null!;
        public string SenderType { get; set; } = null!;
        public Guid? SenderAdminUserId { get; set; }
        public string SenderName { get; set; } = null!;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }
}
