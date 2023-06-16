using SMART.ERP.Application.DTOs.Meta.Payload.PayloadText;

namespace SMART.ERP.Application.DTOs.Meta.Payload
{
    public class MetaValue
    {
        public string? messaging_product { get; set; }
        public MetaMetadata? metadata { get; set; }
        public List<MetaContacts>? contacts { get; set; }
        public List<MetaMessages>? messages { get; set; }
        public List<MetaStatus>? statuses { get; set; }
    }
}
