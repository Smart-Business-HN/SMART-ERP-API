namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class WorkingConditionRequestDto
    {
        public string OrgId { get; set; } = null!;
        public string TenantId { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;
        public int Source { get; set; }
        public string UserId { get; set; } = null!;
    }
}
