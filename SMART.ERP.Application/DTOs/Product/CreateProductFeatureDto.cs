namespace SMART.ERP.Application.DTOs.Product
{
    public class CreateProductFeatureDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
