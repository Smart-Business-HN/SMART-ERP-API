using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class VirtualStockImportDetail
    {
        public int Id { get; init; }

        public int VirtualStockImportId { get; set; }
        public virtual VirtualStockImport? VirtualStockImport { get; set; }

        public int? ProductId { get; set; }
        public virtual Product? Product { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string ProductCode { get; set; } = null!;

        [Precision(18, 2)]
        public decimal Quantity { get; set; }

        [Precision(18, 2)]
        public decimal? CostPrice { get; set; }

        public bool WasSuccessful { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string? ErrorMessage { get; set; }
    }
}
