namespace SMART.ERP.Application.DTOs.Status
{
    public class TypeStatusDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public List<StatusDto>? Status { get; set; }
    }
}
