namespace SMART.ERP.Application.DTOs.AssociatedCompany;

public class AssociatedCompanyDto
{
    public int Id { get; set; }
    public Guid EcommerceUserId { get; set; }
    public string Name { get; set; } = null!;
    public string? RTN { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }
    public DateTime CreationDate { get; set; }
    public bool IsActive { get; set; }
}
