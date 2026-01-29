using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class ProductDataSheet
    {
        public int Id { get; init; }
        [Column(TypeName = "varchar(50)")]
        public string Title { get; set; } = null!;
        public int DataSheetId { get; set; }
        public virtual DataSheet? DataSheet { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModificatedBy { get; set; }
    }
}
