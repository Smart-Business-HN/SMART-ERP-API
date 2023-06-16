namespace SMART.ERP.Application.DTOs.Product
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Image { get; set; } = null!;
        public bool IsPartCategory { get; set; }
        public bool IsActive { get; set; }
        public List<SubcategoryDto> Subcategories { get; set; } = new List<SubcategoryDto>();
    }
}
