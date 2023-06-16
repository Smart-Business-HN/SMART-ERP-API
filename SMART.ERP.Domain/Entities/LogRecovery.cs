namespace SMART.ERP.Domain.Entities
{
    public class LogRecovery
    {
        public int Id { get; init; }
        public string Email { get; set; } = null!;
        public int Code { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; }
    }
}
