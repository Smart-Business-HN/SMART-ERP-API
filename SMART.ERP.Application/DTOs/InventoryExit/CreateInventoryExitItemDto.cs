namespace SMART.ERP.Application.DTOs.InventoryExit
{
    public class CreateInventoryExitItemDto
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public string? Notes { get; set; }
    }
}
