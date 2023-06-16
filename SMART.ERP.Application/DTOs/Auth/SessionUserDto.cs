namespace SMART.ERP.Application.DTOs.Auth
{
    public class SessionUserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Photo { get; set; }
        public string Role { get; set; } = null!;
        public string Token { get; set; } = null!;
        public int? BranchOfficeId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int MainBranchOfficeId { get; set; }
    }
}
