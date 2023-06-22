using SMART.ERP.Application.DTOs.Cai;

namespace SMART.ERP.Application.DTOs.Company
{
    public class BranchOfficeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public float Lat { get; set; }
        public float Lng { get; set; }
        public bool IsActive { get; set; }
        public bool IsMainBranchOffice { get; set; }
        public List<CaiDto>? Cais { get; set; }
    }
}
