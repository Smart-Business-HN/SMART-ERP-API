namespace SMART.ERP.Application.DTOs.Product
{
    public class ProductFeatureDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
