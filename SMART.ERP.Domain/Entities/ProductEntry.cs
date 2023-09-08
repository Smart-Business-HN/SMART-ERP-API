namespace SMART.ERP.Domain.Entities
{
    public class ProductEntry
    {
        public int Id { get; init; }
        public int InventoryInputId { get; set; }
        public virtual InventoryInput? InventoryInput { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitProductPrice { get; set; }
        public decimal Total { get; set; }
    }
}
