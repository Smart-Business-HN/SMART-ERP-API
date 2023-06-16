namespace SMART.ERP.Application.DTOs.Meta.MetaInteractiveReply.MetaInteractiveListReply
{
    public class MetaInteractiveListAction
    {
        public string? button { get; set; }
        public List<MetaInteractiveSection> sections { get; set; } = new List<MetaInteractiveSection>();
    }
}
