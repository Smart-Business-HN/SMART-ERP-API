namespace SMART.ERP.Application.DTOs.Chat
{
    public class ChatSessionDto
    {
        public int Id { get; set; }
        public string SessionIdentifier { get; set; } = null!;
        public string VisitorName { get; set; } = null!;
        public string? VisitorEmail { get; set; }
        public bool IsAuthenticated { get; set; }
        public Guid? EcommerceUserId { get; set; }
        public Guid? AssignedAdminUserId { get; set; }
        public string? AssignedAdminName { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string? LastMessagePreview { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public int UnreadAdminCount { get; set; }
    }
}
