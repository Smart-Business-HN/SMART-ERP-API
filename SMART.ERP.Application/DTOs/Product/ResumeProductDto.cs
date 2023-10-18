using SMART.ERP.Application.DTOs.Status;

namespace SMART.ERP.Application.DTOs.Product
{
    public class ResumeProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? UrlYoutube { get; set; }
        public BrandDto? Brand { get; set; }
        public StatusDto? Status { get; set; }
        public List<ResumeProductImageDto> ProductImages { get; set; } = new List<ResumeProductImageDto>();
        public List<ResumeProductDataSheetDto> ProductDataSheets { get; set; } = new List<ResumeProductDataSheetDto>();
    }
}
