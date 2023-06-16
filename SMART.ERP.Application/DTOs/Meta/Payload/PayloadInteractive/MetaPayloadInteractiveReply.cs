using SMART.ERP.Application.DTOs.Meta.MetaInteractiveReply.MetaInteractiveButtonReply;

namespace SMART.ERP.Application.DTOs.Meta.Payload.PayloadInteractive
{
    public class MetaPayloadInteractiveReply
    {
        public MetaListReply? list_reply { get; set; }
        public MetaInteractiveButtonReply? button_reply { get; set; }
        public string type { get; set; } = null!;
    }

    public class MetaListReply
    {
        public string id { get; set; } = null!;
        public string title { get; set; } = null!;
        public string? description { get; set; }
    }
}
