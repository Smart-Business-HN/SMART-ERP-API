using Microsoft.EntityFrameworkCore;

namespace SMART.ERP.Domain.Entities
{
    public class ProductEntry
    {
        public int Id { get; init; }
        public int InventoryInputId { get; set; }
        public virtual InventoryInput? InventoryInput { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        [Precision(18, 2)]
        public decimal Quantity { get; set; }
        [Precision(18, 2)]
        public decimal UnitProductPrice { get; set; }
        [Precision(18, 2)]
        public decimal Total { get; set; }
    }
}
