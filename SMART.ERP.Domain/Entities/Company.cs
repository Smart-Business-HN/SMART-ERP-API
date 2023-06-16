namespace SMART.ERP.Domain.Entities
{
    public class Company
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string AboutUs { get; set; } = null!;
        public string? FacebookUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? YoutubeUrl { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        public string? ModificatedBy { get; set; }
        public bool IsActive { get; set; }
        public List<BranchOffices>? BranchOffices { get; set; }
        public List<Opinion>? Opinions { get; set; }
        public List<Banner>? Banners { get; set; }
    }
}
