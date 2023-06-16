namespace SMART.ERP.Application.DTOs.Meta
{
    public class MetaStatus
    {
        public string? Id { get; set; }
        public string Recipient_id { get; set; } = null!;
        public string? Status { get; set; }
        public int Timestamp { get; set; }
        public MetaConversation? Conversation { get; set; }
        public MetaPricing? Pricing { get; set; }
    }
}
