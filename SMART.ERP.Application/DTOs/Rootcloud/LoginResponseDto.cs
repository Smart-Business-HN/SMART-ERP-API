namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class LoginResponseDto
    {
        public string Access_Token { get; set; } = string.Empty;
        public string Refresh_Token { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public LoginUserResponseDto User { get; set; } = new LoginUserResponseDto();
        public LoginUserTenantResponseDto Tenant { get; set; } = new LoginUserTenantResponseDto();
    }
}
