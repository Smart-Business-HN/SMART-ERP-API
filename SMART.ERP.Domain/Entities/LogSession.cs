namespace SMART.ERP.Domain.Entities
{
    public class LogSession
    {
        public int Id { get; init; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string IP { get; set; } = null!;
        public string DeviceInfo { get; set; } = null!;
        public string? Lat { get; set; }
        public string? Lng { get; set; }
        public bool IsActive { get; set; }
    }
}
