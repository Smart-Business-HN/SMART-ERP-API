namespace SMART.ERP.Domain.Entities
{
    public class BranchOffices
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public float Lat { get; set; }
        public float Lng { get; set; }
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
        public virtual Company? Company { get; set; }
        public int? CityId { get; set; }
        public virtual City? City { get; set; }
        public string? MapsId { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        public string? ModificatedBy { get; set; }
        public bool IsMainBranchOffice { get; set; }
    }
}
