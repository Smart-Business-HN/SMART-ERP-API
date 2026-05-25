using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Domain.Entities
{
    public class WarehouseTransfer
    {
        public int Id { get; init; }
        [MaxLength(8)]
        public string? Code { get; set; }
        public int PrefixId { get; set; }
        public virtual Prefix? Prefix { get; set; }
        public DateTime TransferDate { get; set; }
        public WarehouseTransferStatus Status { get; set; }
        public int OriginWarehouseId { get; set; }
        public virtual Warehouse? OriginWarehouse { get; set; }
        public int DestinationWarehouseId { get; set; }
        public virtual Warehouse? DestinationWarehouse { get; set; }
        [MaxLength(500)]
        public string? Notes { get; set; }
        public DateTime? SentDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? SentBy { get; set; }
        public DateTime? ReceivedDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ReceivedBy { get; set; }
        [MaxLength(500)]
        public string? CancellationReason { get; set; }
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModifiedBy { get; set; }
        public List<WarehouseTransferItem>? Items { get; set; }
    }
}
