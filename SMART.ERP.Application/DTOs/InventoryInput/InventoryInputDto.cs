using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.DTOs.InventoryInputType;
using SMART.ERP.Application.DTOs.ProductEntry;
using SMART.ERP.Application.DTOs.Warehouse;

namespace SMART.ERP.Application.DTOs.InventoryInput
{
    public class InventoryInputDto
    {
        public int Id { get; set; }
        public int InventoryInputTypeId { get; set; }
        public virtual InventoryInputTypeDto? InventoryInputType { get; set; }
        public int WarehouseId { get; set; }
        public virtual WarehouseDto? Warehouse { get; set; }
        public int PrefixId { get; set; }
        public virtual PrefixDto? Prefix { get; set; }
        public string? Description { get; set; }
        public int? PurchaseOrderId { get; set; }
        //public virtual PurchaseOrderDto? PurchaseOrder { get; set; }
        public int? ProductReturnId { get; set; }
        //public virtual ProductReturnDto? ProductReturn { get; set; }
        public int? SurplusInventoryId { get; set; }
        //public virtual SurplusInventoryDto { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        public string? ModifiedBy { get; set; }
        public List<ProductEntryDto>? ProductEntries { get; set; }
    }
}
