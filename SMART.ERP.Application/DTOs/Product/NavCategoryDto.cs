namespace SMART.ERP.Application.DTOs.Product
{
    public class NavCategoryDto
    {
        public int Id { get; set; }
        public string Category { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public List<ResumeSubcategoryDto> SubCategories { get; set; } = new List<ResumeSubcategoryDto>();
    }
}
