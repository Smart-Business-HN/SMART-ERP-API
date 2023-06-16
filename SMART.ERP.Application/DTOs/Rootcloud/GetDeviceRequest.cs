namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class GetDeviceRequest
    {
        public string OrgId { get; set; } = null!;
        public string QueryOrgIds { get; set; } = null!;
        public List<GetDeviceTargetRequest> TargetId = new List<GetDeviceTargetRequest>();
        public int Page { get; set; }
        public int Limit { get; set; }
    }
}
