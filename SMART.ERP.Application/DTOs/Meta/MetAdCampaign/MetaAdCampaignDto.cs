namespace SMART.ERP.Application.DTOs.Meta.MetAdCampaign
{
    public class MetaAdCampaignDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Lifetime_Budget { get; set; }
        public DateTime Stop_Time { get; set; }
    }
}
