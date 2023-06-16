using SMART.ERP.Application.DTOs.Status;
using SMART.ERP.Application.DTOs.Product;

namespace SMART.ERP.Application.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; init; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Brochure { get; set; }
        public string? VirtualTour { get; set; }
        public string? UrlYoutube { get; set; }
        public bool IsFatherProduct { get; set; }
        public decimal CostPrice { get; set; }
        public decimal RecomendedSalePrice { get; set; }
        public int MinStock { get; set; }
        public int CurrentStock { get; set; }
        public int BrandId { get; private set; }
        public BrandDto? Brand { get; set; }
        public int UnitOfMeasurementId { get; set; }
        public UnitOfMeasurementDto? UnitOfMeasurement { get; set; }
        public int SubCategoryId { get; set; }
        public SubcategoryDto? SubCategory { get; set; }
        public int StatusId { get; set; }
        public StatusDto? Status { get; set; }
        public int ProviderId { get; set; }
        public ProviderDto? Provider { get; set; }
        public bool IsActive { get; set; }
        public List<ProductDataSheetDto>? ProductDataSheets { get; set; }
        public List<ProductFeatureDto>? ProductFeatures { get; set; }
        public List<ProductImageDto>? ProductImages { get; set; }
    }
}
