namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class LoginUserTenatListResponseDto
    {
        public string TenantName { get; set; } = string.Empty;
        public string CompanyLogo { get; set; } = string.Empty;
        public int TenantId { get; set; }
        public string Company { get; set; } = string.Empty;
    }
}
