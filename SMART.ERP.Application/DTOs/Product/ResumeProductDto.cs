using SMART.ERP.Application.DTOs.Status;
using System.Security.Policy;

namespace SMART.ERP.Application.DTOs.Product
{
    public class ResumeProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? UrlYoutube { get; set; }
        public int SubCategoryId { get; set; }
        public decimal CostPrice { get; set; }
        public SubcategoryDto Subcategory { get; set; } = null!;
        public BrandDto? Brand { get; set; }
        public StatusDto? Status { get; set; }
        public List<ResumeProductImageDto> ProductImages { get; set; } = new List<ResumeProductImageDto>();
        public List<ResumeProductDataSheetDto> ProductDataSheets { get; set; } = new List<ResumeProductDataSheetDto>();
    }
}
