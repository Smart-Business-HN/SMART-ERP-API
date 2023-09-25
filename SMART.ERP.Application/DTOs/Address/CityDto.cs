namespace SMART.ERP.Application.DTOs.Address
{
    public class CityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int DepartmentId { get; set; }
        public DepartmentDto? Department { get; set; }
        public bool IsActive { get; set; }
    }
}
