using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SMART.ERP.Domain.Entities
{
    public class InventoryExitItem
    {
        public int Id { get; init; }
        public int InventoryExitId { get; set; }
        public virtual InventoryExit? InventoryExit { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        [Precision(18, 2)]
        public decimal Quantity { get; set; }
        [Precision(18, 4)]
        public decimal? UnitCost { get; set; }
        [MaxLength(500)]
        public string? Notes { get; set; }
        [Precision(18, 2)]
        public decimal? PreviousStock { get; set; }
    }
}
