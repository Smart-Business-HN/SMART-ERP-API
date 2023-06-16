namespace SMART.ERP.Application.DTOs.Meta.MetaInteractiveReply.MetaInteractiveButtonReply
{
    public class MetaInteractiveButtonRoot
    {
        public string messaging_product { get; set; } = "whatsapp";
        public string recipient_type { get; set; } = "individual";
        public string? to { get; set; }
        public string type { get; set; } = "interactive";
        public MetaInteractiveButtonObject interactive { get; set; } = new MetaInteractiveButtonObject();
    }
}
