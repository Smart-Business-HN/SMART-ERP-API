using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.DTOs.User;

namespace SMART.ERP.Application.DTOs.AdvisorDepartment
{
    public class AdvisorDepartmentDto
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public UserDto? User { get; set; }
        public int DepartmentId { get; set; }
        public DepartmentDto? Department { get; set; }
    }
}
