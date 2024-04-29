namespace SMART.ERP.Domain.Entities
{
    public class InventoryInput
    {
        public int Id { get; init; }
        public int InventoryInputTypeId { get; set; }
        public string Code { get; set; } = null!;
        public virtual InventoryInputType? InventoryInputType { get; set; }
        public int WarehouseId { get; set; }
        public virtual Warehouse? Warehouse { get; set; }
        public int PrefixId { get; set; }
        public virtual Prefix? Prefix { get; set; }
        public string? Description { get; set; }
        public int? PurchaseOrderOriginId { get; set; }
        public virtual PurchaseOrder? PurchaseOrderOrigin { get; set; }
        public int? ProductReturnId { get; set; }
        //public virtual ProductReturn? ProductReturn { get; set; }
        public int? SurplusInventoryId { get; set; }
        //public virtual SurplusInventoryId { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        public string? ModifiedBy { get; set; }
        public List<ProductEntry>? ProductEntries { get; set; }
        public int StatusId { get; set; }
        public virtual Status? Status { get; set; }
    }
}
