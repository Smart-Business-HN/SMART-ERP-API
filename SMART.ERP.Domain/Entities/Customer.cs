using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; init; }
        [MaxLength(100)]
        public string FullName { get; set; } = null!;
        [MaxLength(50)]
        public string? FirstName { get; set; }
        [MaxLength(50)]
        public string? LastName { get; set; }
        [MaxLength(50)]
        public string? DNI { get; set; }
        [MaxLength(50)]
        public string? RTN { get; set; }
        [MaxLength(100)]
        public string? Company { get; set; }
        public DateTime? ConstitutionDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? Age { get; set; }
        [MaxLength(70)]
        public string? Email { get; set; }
        public bool ConfirmedEmail { get; set; }
        [MaxLength(70)]
        public string? SecondaryEmail { get; set; }
        [MaxLength(50)]
        public string? PhoneNumber { get; set; }
        public bool ConfirmedPhoneNumber { get; set; }
        [MaxLength(50)]
        public string? SecondaryPhoneNumber { get; set; } = null!;
        public string? Avatar { get; set; }
        [MaxLength(50)]
        public string? CivilStatus { get; set; }
        public byte[]? PasswordHash { get; set; } = null!;
        public byte[]? PasswordSalt { get; set; } = null!;
        public int CustomerTypeId { get; set; }
        public virtual CustomerType? CustomerType { get; set; }
        public int? PriceListId { get; set; }
        public virtual PriceList? PriceList { get; set; }
        public int? GenderId { get; set; }
        public virtual Gender? Gender { get; set; }
        public int? CurrencyId { get; set; }
        public virtual Currency? Currency { get; set; }
        public int? SocialReasonId { get; set; }
        public virtual SocialReason? SocialReason { get; set; }
        public int HeadingId { get; set; }
        public virtual Heading? Heading { get; set; }
        public int? CountryId { get; set; }
        public virtual Country? Country { get; set; }
        public int? DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        public bool IsHisOwnContactPerson { get; set; }
        [MaxLength(50)]
        public string? ContactPerson { get; set; }
        [MaxLength(50)]
        public string? ContactPersonPhone { get; set; }
        [MaxLength(50)]
        public string? ContactPersonEmail { get; set; }
        public List<DeliveryDirection>? DeliveryDirections { get; set; }
        public bool IsActive { get; set; }
        public Guid? UserId { get; set; }
        public virtual User? User { get; set; }
        public bool HasChangedPassword { get; set; }
        public bool HasEcommercePorfile { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool CreditEnabled { get; set; }
        [Precision(18, 2)]
        public decimal CreditLimit { get; set; }
        public List<Invoice>? PendingInvoices { get; set; }
    }
}
