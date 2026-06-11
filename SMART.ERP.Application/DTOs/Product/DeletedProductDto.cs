using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.DTOs.Product
{
    /// <summary>
    /// Vista ligera para la "papelera" de productos del admin: lo necesario para
    /// identificar y restaurar un producto eliminado, mas la metadata del borrado.
    /// </summary>
    public class DeletedProductDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string Name { get; set; } = null!;
        public ProductType ProductType { get; set; }
        public decimal CostPrice { get; set; }
        public decimal RecomendedSalePrice { get; set; }
        public int CurrentStock { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public List<ProductImageDto>? ProductImages { get; set; }
    }
}
