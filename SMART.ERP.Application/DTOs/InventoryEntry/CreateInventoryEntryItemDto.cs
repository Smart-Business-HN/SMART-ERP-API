namespace SMART.ERP.Application.DTOs.InventoryEntry
{
    public class CreateInventoryEntryItemDto
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal? UnitCost { get; set; }
        public string? Notes { get; set; }
    }
}
