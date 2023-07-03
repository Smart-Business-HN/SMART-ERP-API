namespace SMART.ERP.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; init; }
        public string FullName { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DNI { get; set; }
        public string? RTN { get; set; }
        public string? Company { get; set; }
        public DateTime? ConstitutionDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? Age { get; set; }
        public string? Email { get; set; }
        public bool ConfirmedEmail { get; set; }
        public string? SecondaryEmail { get; set; }
        public string? PhoneNumber { get; set; }
        public bool ConfirmedPhoneNumber { get; set; }
        public string? SecondaryPhoneNumber { get; set; } = null!;
        public string? Avatar { get; set; }
        public string? CivilStatus { get; set; }
        public byte[]? PasswordHash { get; set; } = null!;
        public byte[]? PasswordSalt { get; set; } = null!;
        public int CustomerTypeId { get; set; }
        public virtual CustomerType? CustomerType { get; }
        public int? GenderId { get; set; }
        public virtual Gender? Gender { get; }
        public int? CurrencyId { get; set; }
        public virtual Currency? Currency { get; }
        public int? SocialReasonId { get; set; }
        public virtual SocialReason? SocialReason { get; set; }
        public int HeadingId { get; set; }
        public virtual Heading? Heading { get; set; }
        public int? CountryId { get; set; }
        public virtual Country? Country { get; set; }
        public int? DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        public bool IsHisOwnContactPerson { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPersonPhone { get; set; }
        public string? ContactPersonEmail { get; set; }
        public List<DeliveryDirection>? DeliveryDirections { get; set; }
        public bool IsActive { get; set; }
        public Guid? UserId { get; set; }
        public virtual User? User { get; set; }
        public bool HasChangedPassword { get; set; }
        public bool HasEcommercePorfile { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
