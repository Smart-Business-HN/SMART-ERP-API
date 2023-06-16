namespace SMART.ERP.Domain.Entities
{
    public class Prospect
    {
        public Guid Id { get; init; }
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Email { get; set; }
        public string HeadingName { get; set; } = null!;
        public int TypeOriginId { get; set; }
        public virtual TypeOrigin? TypeOrigin { get; set; }
        public string SocialReasonName { get; set; } = null!;
        public int ProspectStepId { get; set; }
        public virtual ProspectStep? ProspectStep { get; set; }
        public int DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        public int CountryId { get; set; }
        public virtual Country? Country { get; set; }
        public int? CityId { get; set; }
        public virtual City? City { get; set; }
        public string? Address { get; set; }
        public string? Observation { get; set; }
        public string? MetaAdCampaignId { get; set; }
        public virtual MetaAdCampaign? MetaAdCampaign { get; set; }
        public int? PostalCode { get; set; }
        public string? WebsiteUrl { get; set; }
        public int? GenderId { get; set; }
        public virtual Gender? Gender { get; set; }
        public string? PreferredContactMethod { get; set; }
        public bool AccountHN { get; set; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPersonPhone { get; set; }
        public string? ContactPersonEmail { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        public string? ModificatedBy { get; set; }
        public virtual List<ProspectQuoteProduct>? ProspectQuoteProducts { get; set; }
    }
}
