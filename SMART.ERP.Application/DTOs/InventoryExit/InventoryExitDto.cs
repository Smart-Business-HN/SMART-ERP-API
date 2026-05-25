using SMART.ERP.Application.DTOs.Warehouse;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.DTOs.InventoryExit
{
    public class InventoryExitDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public int PrefixId { get; set; }
        public DateTime ExitDate { get; set; }
        public InventoryExitReason ExitReason { get; set; }
        public string? CustomReason { get; set; }
        public InventoryExitStatus Status { get; set; }
        public int WarehouseId { get; set; }
        public WarehouseDto? Warehouse { get; set; }
        public string? Notes { get; set; }
        public string? BeneficiaryName { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public string? ConfirmedBy { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        public string? ModifiedBy { get; set; }
        public List<InventoryExitItemDto>? Items { get; set; }
    }
}
