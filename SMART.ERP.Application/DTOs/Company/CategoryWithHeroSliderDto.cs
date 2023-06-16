namespace SMART.ERP.Application.DTOs.Company
{
    public class CategoryWithHeroSliderDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Image { get; set; } = null!;
        public int Position { get; set; }
        public List<HeroSliderDto> HeroSliders { get; set; } = new List<HeroSliderDto>();
    }
}
