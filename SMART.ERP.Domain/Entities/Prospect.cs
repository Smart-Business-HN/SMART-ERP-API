using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class Prospect
    {
        public Guid Id { get; init; }
        [Column(TypeName = "varchar(150)")]
        public string FullName { get; set; } = null!;
        [Column(TypeName = "varchar(15)")]
        public string PhoneNumber { get; set; } = null!;
        [Column(TypeName = "varchar(50)")]
        public string? Email { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string HeadingName { get; set; } = null!;
        public int TypeOriginId { get; set; }
        public virtual TypeOrigin? TypeOrigin { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string SocialReasonName { get; set; } = null!;
        public int ProspectStepId { get; set; }
        public virtual ProspectStep? ProspectStep { get; set; }
        public int DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        public int CountryId { get; set; }
        public virtual Country? Country { get; set; }
        public int? CityId { get; set; }
        public virtual City? City { get; set; }
        [Column(TypeName = "varchar(150)")]
        public string? Address { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string? Observation { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? MetaAdCampaignId { get; set; }
        public virtual MetaAdCampaign? MetaAdCampaign { get; set; }
        public int? PostalCode { get; set; }
        [Column(TypeName = "varchar(max)")]
        public string? WebsiteUrl { get; set; }
        public int? GenderId { get; set; }
        public virtual Gender? Gender { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? PreferredContactMethod { get; set; }
        public bool AccountHN { get; set; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ContactPerson { get; set; }
        [Column(TypeName = "varchar(15)")]
        public string? ContactPersonPhone { get; set; }
        [Column(TypeName = "varchar(30)")]
        public string? ContactPersonEmail { get; set; }
        [Column(TypeName = "datetime2(0)")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(150)")]
        public string CreatedBy { get; set; } = null!;
        [Column(TypeName = "datetime2(0)")]
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(150)")]
        public string? ModificatedBy { get; set; }
        public virtual List<ProspectQuoteProduct>? ProspectQuoteProducts { get; set; }
    }
}
