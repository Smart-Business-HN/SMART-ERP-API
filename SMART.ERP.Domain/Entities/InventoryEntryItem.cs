using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SMART.ERP.Domain.Entities
{
    public class InventoryEntryItem
    {
        public int Id { get; init; }
        public int InventoryEntryId { get; set; }
        public virtual InventoryEntry? InventoryEntry { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        [Precision(18, 2)]
        public decimal Quantity { get; set; }
        [Precision(18, 2)]
        public decimal? UnitCost { get; set; }
        [Precision(18, 2)]
        public decimal Total { get; set; }
        [MaxLength(500)]
        public string? Notes { get; set; }
        [Precision(18, 2)]
        public decimal? PreviousCostPrice { get; set; }
        [Precision(18, 2)]
        public decimal? PreviousStock { get; set; }
    }
}
