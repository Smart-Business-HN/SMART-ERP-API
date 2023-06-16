namespace SMART.ERP.Application.DTOs.Product
{
    public class ProviderDto
    {
        public int Id { get; init; }
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
    }
}
