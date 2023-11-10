namespace SMART.ERP.Application.DTOs.Product
{
    public class BasicDetailProductDto
    {
        public string Code { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal RecomendedSalePrice { get; set; }
        public int BrandId { get; set; }
        public BrandDto? Brand { get; set; }
        public int SubCategoryId { get; set; }
        public SubcategoryDto? SubCategory { get; set; }
        public List<ProductImageDto>? ProductImages { get; set; }
    }
}
