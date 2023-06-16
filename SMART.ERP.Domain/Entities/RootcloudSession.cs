namespace SMART.ERP.Domain.Entities
{
    public class RootcloudSession
    {
        public int Id { get; init; }
        public string TicketId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string OrgIds { get; set; } = null!;
        public int TenantId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; }
    }
}
