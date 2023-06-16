namespace SMART.ERP.Application.DTOs.Meta.MetaInteractiveReply.MetaInteractiveListReply
{
    public class MetaInteractiveObject
    {
        public string? type { get; set; }
        public MetaInteractiveHeader header { get; set; } = new MetaInteractiveHeader();
        public MetaInteractiveBody body { get; set; } = new MetaInteractiveBody();
        public MetaInteractiveFooter? footer { get; set; }
        public MetaInteractiveListAction action { get; set; } = new MetaInteractiveListAction();
    }
}
