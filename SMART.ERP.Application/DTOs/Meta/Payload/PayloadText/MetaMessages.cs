using SMART.ERP.Application.DTOs.Meta.Payload.PayloadButton;
using SMART.ERP.Application.DTOs.Meta.Payload.PayloadInteractive;
using SMART.ERP.Application.DTOs.Meta.Payload.PayloadReaction;

namespace SMART.ERP.Application.DTOs.Meta.Payload.PayloadText
{
    public class MetaMessages
    {
        public string From { get; set; } = null!;
        public string? Id { get; set; }
        public int Timestamp { get; set; }
        public MetaContext? context { get; set; }
        public MetaText? Text { get; set; }
        public MetaReaction? Reaction { get; set; }
        public MetaButton? Button { get; set; }
        public MetaPayloadInteractiveReply? Interactive { get; set; }
        public string? Type { get; set; }
    }
}
