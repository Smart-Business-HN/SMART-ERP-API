namespace SMART.ERP.Application.DTOs.User
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Selector { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
