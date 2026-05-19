using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.DTOs.User;

namespace SMART.ERP.Application.DTOs.Customer
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DNI { get; set; }
        public string? RTN { get; set; }
        public string? Company { get; set; }
        public DateTime? ConstitutionDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public int Age { get; set; }
        public string Email { get; set; } = null!;
        public bool ConfirmedEmail { get; set; }
        public string? SecondaryEmail { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public bool ConfirmedPhoneNumber { get; set; }
        public string? SecondaryPhoneNumber { get; set; }
        public string? CivilStatus { get; set; }
        public int CustomerTypeId { get; set; }
        public CustomerTypeDto? CustomerType { get; set; }
        public int GenderId { get; set; }
        public GenderDto? Gender { get; set; }
        public int SocialReasonId { get; set; }
        public SocialReasonDto? SocialReason { get; set; }
        public int HeadingId { get; set; }
        public HeadingDto? Heading { get; set; }
        public int? CountryId { get; set; }
        public CountryDto? Country { get; set; }
        public int? DepartmentId { get; set; }
        public DepartmentDto? Department { get; set; }
        public int CurrencyId { get; set; }
        public bool IsHisOwnContactPerson { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPersonPhone { get; set; }
        public string? ContactPersonEmail { get; set; }
        public List<DeliveryDirectionDto>? DeliveryDirections { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
