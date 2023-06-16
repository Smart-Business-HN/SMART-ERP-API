namespace SMART.ERP.Application.DTOs.Meta.MetaInteractiveReply.MetaInteractiveListReply
{
    public class MetaInteractive
    {
        public string messaging_product { get; set; } = "whatsapp";
        public string recipient_type { get; set; } = "individual";
        public string? to { get; set; }
        public string type { get; set; } = "interactive";
        public MetaInteractiveObject interactive { get; set; } = new MetaInteractiveObject();
    }
}
