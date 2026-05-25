using SMART.ERP.Application.DTOs.Warehouse;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.DTOs.WarehouseTransfer
{
    public class WarehouseTransferDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public int PrefixId { get; set; }
        public DateTime TransferDate { get; set; }
        public WarehouseTransferStatus Status { get; set; }
        public int OriginWarehouseId { get; set; }
        public WarehouseDto? OriginWarehouse { get; set; }
        public int DestinationWarehouseId { get; set; }
        public WarehouseDto? DestinationWarehouse { get; set; }
        public string? Notes { get; set; }
        public DateTime? SentDate { get; set; }
        public string? SentBy { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string? ReceivedBy { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public List<WarehouseTransferItemDto>? Items { get; set; }
    }
}
