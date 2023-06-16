namespace SMART.ERP.Application.DTOs.Meta.MetaInteractiveReply.MetaInteractiveButtonReply
{
    public class MetaInteractiveButton
    {
        public string type { get; set; } = "reply";
        public MetaInteractiveButtonReply reply { get; set; } = new MetaInteractiveButtonReply();
    }

    public class MetaInteractiveButtonReply
    {
        public string? id { get; set; }
        public string? title { get; set; }
    }
}
