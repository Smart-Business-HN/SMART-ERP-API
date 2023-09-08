namespace SMART.ERP.Domain.Entities
{
    public class InventoryDistribution
    {
        public int Id { get; init; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        public int WarehouseId { get; set; }
        public virtual Warehouse Warehouse { get; set; } = null!;
        public double Quantity { get; set; }
    }
}
