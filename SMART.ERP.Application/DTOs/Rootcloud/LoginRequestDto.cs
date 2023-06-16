namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class LoginRequestDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Type { get; set; }
        public bool Encrypted { get; set; }
    }
}
