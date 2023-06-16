namespace SMART.ERP.Application.DTOs.Meta
{
    public class MetaConversation
    {
        public string? Id { get; set; }
        public int Expiration_timestamp { get; set; }
        public MetaOrigin? Origin { get; set; }
    }
}
