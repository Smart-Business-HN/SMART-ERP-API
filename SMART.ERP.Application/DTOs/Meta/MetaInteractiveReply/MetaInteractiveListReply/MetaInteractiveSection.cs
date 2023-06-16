namespace SMART.ERP.Application.DTOs.Meta.MetaInteractiveReply.MetaInteractiveListReply
{
    public class MetaInteractiveSection
    {
        public string? title { get; set; }
        public List<MetaInteractiveRow> rows { get; set; } = new List<MetaInteractiveRow>();
    }
}
