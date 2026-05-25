namespace SMART.ERP.Application.DTOs.WarehouseTransfer
{
    public class WarehouseTransferItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal? UnitCost { get; set; }
    }
}
