using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.DTOs.Company;

namespace SMART.ERP.Application.DTOs.User
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Photo { get; set; }
        public string Email { get; set; } = null!;
        public bool ConfirmedEmail { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public bool ConfirmedPhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
        public RoleDto? Role { get; set; }
        public int GenderId { get; set; }
        public GenderDto? Gender { get; set; }
        public int? CityId { get; set; }
        public CityDto? City { get; set; }
        public int? BranchOfficeId { get; set; }
        public BranchOfficeDto? BranchOffice { get; set; }
        public decimal? SalesGoal { get; set; }
    }
}
