using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities;

public class EcommerceUser
{
    public Guid Id { get; init; }
    [MaxLength(100)]
    public string Email { get; set; } = null!;
    [MaxLength(50)]
    public string UserName { get; set; } = null!;
    [MaxLength(100)]
    public string FullName { get; set; } = null!;
    [MaxLength(50)]
    public string FirstName { get; set; } = null!;
    [MaxLength(50)]
    public string LastName { get; set; } = null!;
    public string? Photo { get; set; }
    [MaxLength(14)]
    public string PhoneNumber { get; set; } = null!;
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public int GenderId { get; set; }
    public virtual Gender? Gender { get; set; }
    public DateTime CreationDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime? BirthDay { get; set; }
    // Nulas para las cuentas creadas con un proveedor externo (Google), que no tienen contraseña.
    public byte[]? PasswordHash { get; set; }
    public byte[]? PasswordSalt { get; set; }
    public byte[]? MasterPasswordHash { get; set; }
    public byte[]? MasterPasswordSalt { get; set; }
    public DateTime? LastPasswordChange { get; set; }
    public DateTime? ModificationDate { get; set; }
    public int CustomerTypeId { get; set; }
    public CustomerType? CustomerType { get; set; }
    public ICollection<Cart>? Carts { get; set; }

    /// <summary>Proveedor con el que se creo la cuenta. Ver <see cref="Enums.AuthProvider"/>.</summary>
    public int AuthProvider { get; set; } = (int)Enums.AuthProvider.Local;

    /// <summary>Claim "sub" de Google. Identificador estable, a diferencia del correo.</summary>
    [MaxLength(64)]
    public string? GoogleSubjectId { get; set; }

    /// <summary>True cuando la propiedad del correo fue probada por un proveedor externo.</summary>
    public bool EmailVerified { get; set; }
}
