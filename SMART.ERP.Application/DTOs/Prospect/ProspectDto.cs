using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.DTOs.Meta.MetAdCampaign;
using SMART.ERP.Application.DTOs.ProspectQuoteProduct;
using SMART.ERP.Application.DTOs.User;

namespace SMART.ERP.Application.DTOs.Prospect
{
    public class ProspectDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Email { get; set; }
        public string HeadingName { get; set; } = null!;
        public int TypeOriginId { get; set; }
        public string SocialReasonName { get; set; } = null!;
        public int CountryId { get; set; }
        public int? CityId { get; set; }
        public string? Address { get; set; }
        public string? Observation { get; set; }
        public string? MetaAdCampaignId { get; set; }
        public MetaAdCampaignDto? MetaAdCampaign { get; set; }
        public int? PostalCode { get; set; }
        public string? WebsiteUrl { get; set; }
        public int? GenderId { get; set; }
        public string? PreferredContactMethod { get; set; }
        public bool AccountHN { get; set; }
        public int DepartmentId { get; set; }
        public DepartmentDto? Department { get; set; }
        public int ProspectStepId { get; set; }
        public ProspectStepDto? ProspectStep { get; set; }
        public Guid UserId { get; set; }
        public UserDto? User { get; set; }
        public List<ProspectQuoteProductDto>? ProspectQuoteProducts { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPersonPhone { get; set; }
        public string? ContactPersonEmail { get; set; }
        public int? OpportunityId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
    }
}
