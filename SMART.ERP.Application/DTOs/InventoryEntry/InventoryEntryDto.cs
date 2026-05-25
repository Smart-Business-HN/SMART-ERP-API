using SMART.ERP.Application.DTOs.Warehouse;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.DTOs.InventoryEntry
{
    public class InventoryEntryDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public int PrefixId { get; set; }
        public InventoryEntryType EntryType { get; set; }
        public InventoryEntryStatus Status { get; set; }
        public DateTime EntryDate { get; set; }
        public int WarehouseId { get; set; }
        public WarehouseDto? Warehouse { get; set; }
        public int? ProviderId { get; set; }
        public string? ProviderName { get; set; }
        public string? SupplierReference { get; set; }
        public string? Description { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public string? ConfirmedBy { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        public string? ModifiedBy { get; set; }
        public List<InventoryEntryItemDto>? Items { get; set; }
    }
}
