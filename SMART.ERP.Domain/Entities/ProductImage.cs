using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class ProductImage
    {
        public int Id { get; init; }
        [Column(TypeName = "varchar(400)")]
        public string FileName { get; set; } = null!;
        [Column(TypeName = "varchar(max)")]
        public string Url { get; set; } = null!;
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
    }
}
