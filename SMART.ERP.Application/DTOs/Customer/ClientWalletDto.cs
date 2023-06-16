using SMART.ERP.Application.DTOs.Opportunity;

namespace SMART.ERP.Application.DTOs.Customer
{
    public class ClientWalletDto
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DNI { get; set; }
        public string? RTN { get; set; }
        public string? Company { get; set; }
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? SecondaryPhoneNumber { get; set; }
        public int SocialReasonId { get; set; }
        public SocialReasonDto? SocialReason { get; set; }
        public int HeadingId { get; set; }
        public HeadingDto? Heading { get; set; }
        public bool IsHisOwnContactPerson { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPersonPhone { get; set; }
        public string? ContactPersonEmail { get; set; }
        public int NumOpportunities { get; set; }
        public int NumProducts { get; set; }
        public bool Assigned { get; set; }
        public Guid? UserId { get; set; }
        public List<OpportunityWalletDto>? Opportunities { get; set; } = new List<OpportunityWalletDto>();
    }
}
