namespace SMART.ERP.Application.DTOs.Auth
{
    public class RecoveryCodeDto
    {
        public Guid Id { get; set; }
        public int Code { get; set; }
        public DateTime Expiration { get; set; }
    }
}
