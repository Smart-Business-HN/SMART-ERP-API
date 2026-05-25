namespace SMART.ERP.Application.DTOs.Kardex
{
    public class KardexReportDto
    {
        public int ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public int? WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public decimal OpeningQuantity { get; set; }
        public decimal OpeningAverageCost { get; set; }
        public decimal OpeningTotalValue { get; set; }

        public decimal TotalIn { get; set; }
        public decimal TotalOut { get; set; }
        public decimal ClosingQuantity { get; set; }
        public decimal ClosingAverageCost { get; set; }
        public decimal ClosingTotalValue { get; set; }

        public List<KardexMovementDto> Movements { get; set; } = [];
    }
}
