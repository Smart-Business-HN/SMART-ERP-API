namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class LoginUserResponseDto
    {
        public bool FirstLogin { get; set; }
        public string Id { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string SuperAdminUserId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool Accepted { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsSystemAdmin { get; set; }
        public string OrgIds { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsSystemUser { get; set; }
        public string Department { get; set; } = string.Empty;
        public int SuperAdmin { get; set; }
        public string TicketId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<LoginUserTenatListResponseDto> TenatList { get; set; } = new List<LoginUserTenatListResponseDto>();
    }
}
