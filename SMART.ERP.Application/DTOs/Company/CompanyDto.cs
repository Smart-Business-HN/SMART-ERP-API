using SMART.ERP.Application.DTOs.Cai;

namespace SMART.ERP.Application.DTOs.Company
{
    public class CompanyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string AboutUs { get; set; } = null!;
        public string FacebookUrl { get; set; } = null!;
        public string TwitterUrl { get; set; } = null!;
        public string InstagramUrl { get; set; } = null!;
        public string YoutubeUrl { get; set; } = null!;
        public int TypeEntityId { get; set; }
        public bool IsActive { get; set; }
        public int? CaiId { get; set; }
        public CaiDto? Cai { get; set; }
        public List<BranchOfficeDto>? BranchOffices { get; set; }
        public List<BannerDto>? Banners { get; set; }
        public List<OpinionDto>? Opinions { get; set; }
    }
}
