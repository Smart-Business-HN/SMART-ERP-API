using SMART.MASTER.Domain.Entities;

namespace SMART.ERP.Application.DTOs.Address
{
    public class ClientDepartmentDto
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public List<ClientCityDto>? Cities { get; set; }
    }
}
