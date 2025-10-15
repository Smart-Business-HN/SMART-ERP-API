namespace SMART.ERP.Domain.Entities;

public class EcommerceUser
{
    public Guid Id { get; init; }
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Photo { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public int GenderId { get; set; }
    public virtual Gender? Gender { get; set; }
    public DateTime CreationDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime? BirthDay { get; set; }
    public byte[] PasswordHash { get; set; } = null!;
    public byte[] PasswordSalt { get; set; } = null!;
    public byte[] MasterPasswordHash { get; set; } = null!;
    public byte[] MasterPasswordSalt { get; set; } = null!;
    public DateTime? LastPasswordChange { get; set; }
    public DateTime? ModificationDate { get; set; }
    public int CustomerTypeId { get; set; }
    public CustomerType? CustomerType { get; set; }
    public ICollection<Cart>? Carts { get; set; }
}