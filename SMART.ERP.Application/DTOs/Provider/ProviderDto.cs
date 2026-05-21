using SMART.ERP.Application.DTOs.TypeProvider;

namespace SMART.ERP.Application.DTOs.Provider
{
    public class ProviderDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string RTN { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? ContactPerson { get; set; }
        public string? ContactPhoneNumber { get; set; }
        public string? ContactEmail { get; set; }
        public string Address { get; set; } = null!;
        public string? WebsiteUrl { get; set; }
        public bool IsActive { get; set; }
        public int TypeProviderId { get; set; }
        public TypeProviderDto TypeProvider { get; set; } = null!;
        public bool CreditEnabled { get; set; }
        public decimal CreditLimit { get; set; }
    }
}
