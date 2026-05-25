using Microsoft.EntityFrameworkCore;

namespace SMART.ERP.Domain.Entities
{
    public class WarehouseTransferItem
    {
        public int Id { get; init; }
        public int WarehouseTransferId { get; set; }
        public virtual WarehouseTransfer? WarehouseTransfer { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        [Precision(18, 2)]
        public decimal Quantity { get; set; }
        [Precision(18, 4)]
        public decimal? UnitCost { get; set; }
    }
}
