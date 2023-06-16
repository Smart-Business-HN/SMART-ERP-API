namespace SMART.ERP.Application.DTOs.Meta.Payload
{
    public class MetaEntry
    {
        public string id { get; set; } = null!;
        public List<MetaChanges> changes { get; set; } = null!;
    }
}
