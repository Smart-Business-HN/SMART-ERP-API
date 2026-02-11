using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities;

public class AssociatedCompany
{
    public int Id { get; init; }
    public Guid EcommerceUserId { get; set; }
    public virtual EcommerceUser? EcommerceUser { get; set; }
    [MaxLength(100)]
    public string Name { get; set; } = null!;
    [MaxLength(20)]
    public string? RTN { get; set; }
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
    [MaxLength(300)]
    public string? Address { get; set; }
    [MaxLength(100)]
    public string? Email { get; set; }
    public DateTime CreationDate { get; set; }
    public bool IsActive { get; set; }
}
