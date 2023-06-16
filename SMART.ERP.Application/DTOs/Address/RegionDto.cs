namespace SMART.ERP.Application.DTOs.Address
{
    public class RegionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int CountryId { get; set; }
        public bool IsActive { get; set; }
        public List<DepartmentDto>? Departments { get; set; }
    }
}
