using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.DTOs.User;

namespace SMART.ERP.Application.DTOs.Warehouse
{
    public class WarehouseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public Guid? UserId { get; set; }
        public UserDto? User { get; set; }
        public int BranchOfficeId { get; set; }
        public BranchOfficeDto? BranchOffice { get; set; }
        public bool IsGeneralWarehouse { get; set; }
        public int? CityId { get; set; }
        public CityDto? City { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        public string? ModificatedBy { get; set; }
    }
}
