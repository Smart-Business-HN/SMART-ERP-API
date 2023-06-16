using SMART.MASTER.Domain.Entities;

namespace SMART.ERP.Application.DTOs.Address
{
    public class ClientCountryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public string CountryCode { get; set; } = null!;
        public List<ClientDepartmentDto>? Departments { get; set; }
    }
}
