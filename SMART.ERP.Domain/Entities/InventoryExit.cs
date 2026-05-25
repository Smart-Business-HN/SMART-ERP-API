using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Domain.Entities
{
    public class InventoryExit
    {
        public int Id { get; init; }
        [MaxLength(8)]
        public string? Code { get; set; }
        public int PrefixId { get; set; }
        public virtual Prefix? Prefix { get; set; }
        public DateTime ExitDate { get; set; }
        public InventoryExitReason ExitReason { get; set; }
        [MaxLength(255)]
        public string? CustomReason { get; set; }
        public InventoryExitStatus Status { get; set; }
        public int WarehouseId { get; set; }
        public virtual Warehouse? Warehouse { get; set; }
        [MaxLength(2000)]
        public string? Notes { get; set; }
        [MaxLength(255)]
        public string? BeneficiaryName { get; set; }
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
        public List<InventoryExitItem>? Items { get; set; }
    }
}
