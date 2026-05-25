using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Domain.Entities
{
    public class InventoryMovement
    {
        public int Id { get; init; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public int WarehouseId { get; set; }
        public virtual Warehouse? Warehouse { get; set; }
        public DateTime MovementDate { get; set; }
        public KardexMovementType MovementType { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string DocumentType { get; set; } = null!;
        public int DocumentId { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? DocumentCode { get; set; }
        [MaxLength(255)]
        public string? ThirdPartyName { get; set; }
        [Precision(18, 4)]
        public decimal QuantityIn { get; set; }
        [Precision(18, 4)]
        public decimal QuantityOut { get; set; }
        [Precision(18, 4)]
        public decimal? UnitCost { get; set; }
        [Precision(18, 2)]
        public decimal? TotalCost { get; set; }
        [Precision(18, 4)]
        public decimal RunningQuantity { get; set; }
        [Precision(18, 4)]
        public decimal RunningAverageCost { get; set; }
        [Precision(18, 2)]
        public decimal RunningTotalValue { get; set; }
        public Guid? UserId { get; set; }
        [MaxLength(255)]
        public string? UserName { get; set; }
        [MaxLength(500)]
        public string? Notes { get; set; }
        public bool IsCancellation { get; set; }
        public int? CancelledMovementId { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
