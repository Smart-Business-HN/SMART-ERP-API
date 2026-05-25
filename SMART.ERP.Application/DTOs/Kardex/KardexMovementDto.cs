using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.DTOs.Kardex
{
    public class KardexMovementDto
    {
        public int Id { get; set; }
        public DateTime MovementDate { get; set; }
        public KardexMovementType MovementType { get; set; }
        public string MovementTypeName { get; set; } = null!;
        public string DocumentType { get; set; } = null!;
        public string? DocumentCode { get; set; }
        public string? ThirdPartyName { get; set; }
        public decimal QuantityIn { get; set; }
        public decimal QuantityOut { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? TotalCost { get; set; }
        public decimal RunningQuantity { get; set; }
        public decimal RunningAverageCost { get; set; }
        public decimal RunningTotalValue { get; set; }
        public string? UserName { get; set; }
        public string? Notes { get; set; }
        public bool IsCancellation { get; set; }
    }
}
