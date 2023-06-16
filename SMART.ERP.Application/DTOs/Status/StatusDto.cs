namespace SMART.ERP.Application.DTOs.Status
{
    public class StatusDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public int TypeStatusId { get; set; }
        public ResumeTypeStatusDto? TypeStatus { get; set; }
    }
}
