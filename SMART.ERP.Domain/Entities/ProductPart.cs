using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class ProductPart
    {
        public int Id { get; init; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        [Column(TypeName = "varchar(20)")]
        public string ProductCode { get; set; } = null!;
        [Column(TypeName = "varchar(50)")]
        public string ProductName { get; set; } = null!;
        public int FatherProductId { get; set; }
        public virtual Product FatherProduct { get; set; } = null!;
        [Column(TypeName = "varchar(20)")]
        public string FatherProductCode { get; set; } = null!;
        [Column(TypeName = "varchar(50)")]
        public string FatherProductName { get; set; } = null!;
        [Precision(18, 4)]
        public decimal Quantity { get; set; }
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModificatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
