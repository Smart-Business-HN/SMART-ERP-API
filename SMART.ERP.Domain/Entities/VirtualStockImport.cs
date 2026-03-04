using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class VirtualStockImport
    {
        public int Id { get; init; }

        public int ProviderId { get; set; }
        public virtual Provider? Provider { get; set; }

        public int WarehouseId { get; set; }
        public virtual Warehouse? Warehouse { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string FileName { get; set; } = null!;

        public DateTime ImportDate { get; set; }

        public int TotalProducts { get; set; }
        public int SuccessfulImports { get; set; }
        public int FailedImports { get; set; }

        [Column(TypeName = "varchar(max)")]
        public string? ErrorLog { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string ImportedBy { get; set; } = null!;

        public virtual List<VirtualStockImportDetail>? ImportDetails { get; set; }
    }
}
