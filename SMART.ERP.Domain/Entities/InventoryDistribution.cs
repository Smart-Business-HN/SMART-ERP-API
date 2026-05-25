using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SMART.ERP.Domain.Entities
{
    public class InventoryDistribution
    {
        public int Id { get; init; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public int WarehouseId { get; set; }
        public virtual Warehouse? Warehouse { get; set; }
        [Precision(18, 2)]
        public decimal Quantity { get; set; }
        [Precision(18, 2)]
        public decimal? MinStock { get; set; }
        [Precision(18, 2)]
        public decimal? MaxStock { get; set; }
        public DateTime? CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? CreatedBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModifiedBy { get; set; }
    }
}
