namespace SMART.ERP.Application.DTOs.Meta.MetaTextReply
{
    public class MetaResponse
    {
        public string messaging_product { get; set; } = "whatsapp";
        public string recipient_type { get; set; } = "individual";
        public string? to { get; set; }
        public string type { get; set; } = "text";
        public MetaResponseText text { get; set; } = new MetaResponseText();

    }

    public class MetaResponseText
    {
        public bool preview_url { get; set; }
        public string? body { get; set; }
    }
}
