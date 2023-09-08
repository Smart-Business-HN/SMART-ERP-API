using SMART.ERP.Application.DTOs.InventoryInput;
using SMART.ERP.Application.DTOs.Product;

namespace SMART.ERP.Application.DTOs.ProductEntry
{
    public class ProductEntryDto
    {
        public int? Id { get; set; }
        public int InventoryInputId { get; set; }
        public virtual InventoryInputDto? InventoryInput { get; set; }
        public int ProductId { get; set; }
        public virtual ProductDto? Product { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitProductPrice { get; set; }
        public decimal Total { get; set; }
    }
}
