namespace SMART.ERP.Application.DTOs.Product
{
    public class SubcategoryDto
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public int CategoryId { get; set; }
        public CategoryDto? Category { get; set; }
        public bool IsActive { get; set; }
    }
}
