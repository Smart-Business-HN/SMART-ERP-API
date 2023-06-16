using SMART.ERP.Application.DTOs.Address;

namespace SMART.ERP.Application.DTOs.Customer
{
    public class CountryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public string CountryCode { get; set; } = null!;
        public List<DepartmentDto>? Departments { get; set; }
    }
}
