namespace SMART.ERP.Application.DTOs.Meta.MetAdCampaign
{
    public class MetaAdCampaignReply
    {
        public List<MetaAdCampaignReplyObject>? data { get; set; }
        public MetaAdCampaignPaging? paging { get; set; }

    }

    public class MetaAdCampaignReplyObject
    {
        public string id { get; set; } = null!;
        public string name { get; set; } = null!;
        public string? lifetime_budget { get; set; }
        public string stop_time { get; set; } = null!;
    }

    public class MetaAdCampaignPaging
    {
        public MetaAdCampaignPagingCursors? cursors { get; set; }
        public string? next { get; set; }
    }

    public class MetaAdCampaignPagingCursors
    {
        public string? before { get; set; }
        public string? after { get; set; }
    }
}
