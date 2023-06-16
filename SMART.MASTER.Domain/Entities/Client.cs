namespace SMART.MASTER.Domain.Entities
{
    public class Client
    {
        public Guid Id { get; }
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
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
        public int CustomerTypeId { get; set; }
        public virtual ClientType? CustomerType { get; }
        public int? GenderId { get; set; }
        public virtual ClientGender? Gender { get; }
        public int? CurrencyId { get; set; }
        public virtual ClientCurrency? Currency { get; }
        public int? SocialReasonId { get; set; }
        public virtual ClientSocialReason? SocialReason { get; set; }
        public int HeadingId { get; set; }
        public virtual ClientHeading? Heading { get; set; }
        public int? CountryId { get; set; }
        public virtual ClientCountry? Country { get; set; }
        public int? DepartmentId { get; set; }
        public virtual ClientDepartment? Department { get; set; }
        public bool IsHisOwnContactPerson { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPersonPhone { get; set; }
        public string? ContactPersonEmail { get; set; }
        public List<DeliveryDirection>? DeliveryDirections { get; set; }
        public bool IsActive { get; set; }
        public bool HasChangedPassword { get; set; }
    }
}
