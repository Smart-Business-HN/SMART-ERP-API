namespace SMART.ERP.Domain.Entities
{
    public class Product
    {
        public int Id { get; init; }
        public string Code { get; set; } = null!;
        public string Slug { get; set; } = null!;
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
        public int BrandId { get; set; }
        public virtual Brand? Brand { get; set; }
        public int UnitOfMeasurementId { get; set; }
        public virtual UnitOfMeasurement? UnitOfMeasurement { get; set; }
        public int SubCategoryId { get; set; }
        public virtual Subcategory? SubCategory { get; set; }
        public int StatusId { get; set; }
        public virtual Status? Status { get; set; }
        public int ProviderId { get; set; }
        public virtual Provider? Provider { get; set; }
        public bool IsActive { get; set; }
        public bool ItemInSAP { get; set; }
        public bool InventoryItem { get; set; }
        public bool PurchaseItem { get; set; }
        public bool SalesItem { get; set; }
        public int ItemGroup { get; set; }
        public int ItemSerie { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        public string? ModificatedBy { get; set; }
        public List<ProductDataSheet>? ProductDataSheets { get; set; }
        public List<ProductFeature>? ProductFeatures { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
    }
}
