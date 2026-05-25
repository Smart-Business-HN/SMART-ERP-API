using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Services.InventoryMovementService
{
    /// <summary>
    /// Datos para registrar un movimiento de Kardex. QuantityIn/QuantityOut son deltas
    /// ya calculados por el handler según la semántica del documento (compra=suma,
    /// ajuste=delta absoluto, salida=resta, transferencia=in/out).
    /// </summary>
    public class RecordMovementInput
    {
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public DateTime MovementDate { get; set; }
        public KardexMovementType MovementType { get; set; }
        public string DocumentType { get; set; } = null!;
        public int DocumentId { get; set; }
        public string? DocumentCode { get; set; }
        public string? ThirdPartyName { get; set; }
        public decimal QuantityIn { get; set; }
        public decimal QuantityOut { get; set; }
        public decimal? UnitCost { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Notes { get; set; }

        /// <summary>Si true, actualiza InventoryDistribution.Quantity y Product.CurrentStock.</summary>
        public bool SyncStock { get; set; } = true;

        /// <summary>Si true, actualiza Product.CostPrice con el costo promedio ponderado resultante.</summary>
        public bool UpdateProductCost { get; set; }

        public bool IsCancellation { get; set; }
        public int? CancelledMovementId { get; set; }
    }
}
