using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Domain.Entities
{
    public class InventoryEntry
    {
        public int Id { get; init; }
        [MaxLength(8)]
        public string? Code { get; set; }
        public int PrefixId { get; set; }
        public virtual Prefix? Prefix { get; set; }
        public InventoryEntryType EntryType { get; set; }
        public InventoryEntryStatus Status { get; set; }
        public DateTime EntryDate { get; set; }
        public int WarehouseId { get; set; }
        public virtual Warehouse? Warehouse { get; set; }
        public int? ProviderId { get; set; }
        public virtual Provider? Provider { get; set; }
        public int? PurchaseOrderOriginId { get; set; }
        public virtual PurchaseOrder? PurchaseOrderOrigin { get; set; }
        [MaxLength(100)]
        public string? SupplierReference { get; set; }
        [MaxLength(400)]
        public string? Description { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ConfirmedBy { get; set; }
        [MaxLength(500)]
        public string? CancellationReason { get; set; }
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModifiedBy { get; set; }
        // Proyecto del que proviene el material (obligatorio para EntryType.ProjectSurplus).
        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        public List<InventoryEntryItem>? Items { get; set; }
    }
}
