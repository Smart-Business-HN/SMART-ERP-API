using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class Product
    {
        public int Id { get; init; }
        [Column(TypeName = "varchar(20)")]
        public string Code { get; set; } = null!;
        [MaxLength(1500)]
        public string Slug { get; set; } = null!;
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = null!;
        [Column(TypeName = "varchar(max)")]
        public string? Description { get; set; }
        [Column(TypeName = "varchar(max)")]
        public string? Brochure { get; set; }
        [Column(TypeName = "varchar(max)")]
        public string? VirtualTour { get; set; }
        [Column(TypeName = "varchar(max)")]
        public string? UrlYoutube { get; set; }
        public bool IsFatherProduct { get; set; }
        [Precision(18, 2)]
        public decimal CostPrice { get; set; }
        [Precision(18, 2)]
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
        public bool ShowInEcommerce { get; set; }
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [MaxLength(50)]
        public string? ModificatedBy { get; set; }
        public int TaxId { get; set; }
        public virtual Tax? Tax { get; set; }
        public List<ProductDataSheet>? ProductDataSheets { get; set; }
        public List<ProductFeature>? ProductFeatures { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
        public List<InventoryDistribution>? InventoryDistributions { get; set; }
        public List<ProductPurchasePriceLog>? ProductPurchasePriceLogs { get; set; }
        public string? EcommerceDescription { get; set; }
    }
}
