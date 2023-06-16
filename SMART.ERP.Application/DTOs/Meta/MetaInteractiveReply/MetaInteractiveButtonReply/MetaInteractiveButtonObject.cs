namespace SMART.ERP.Application.DTOs.Meta.MetaInteractiveReply.MetaInteractiveButtonReply
{
    public class MetaInteractiveButtonObject
    {
        public string? type { get; set; }
        public MetaInteractiveButtonHeader header { get; set; } = new MetaInteractiveButtonHeader();
        public MetaInteractiveButtonBody body { get; set; } = new MetaInteractiveButtonBody();
        public MetaInteractiveButtonFooter? footer { get; set; }
        public MetaInteractiveButtonAction action { get; set; } = new MetaInteractiveButtonAction();
    }
}
