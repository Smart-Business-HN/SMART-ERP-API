using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.DTOs.User;

namespace SMART.ERP.Application.DTOs.EcommerceUser;

public class EcommerceUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Photo { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public int? DepartmentId { get; set; }
    public virtual DepartmentDto? Department { get; set; }
    public int GenderId { get; set; }
    public virtual GenderDto? Gender { get; set; }
    public DateTime CreationDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime? BirthDay { get; set; }
    public DateTime? LastPasswordChange { get; set; }
    public DateTime? ModificationDate { get; set; }
    public int CustomerTypeId { get; set; }
    public CustomerTypeDto? CustomerType { get; set; }
}