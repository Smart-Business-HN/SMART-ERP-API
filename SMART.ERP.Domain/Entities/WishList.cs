using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class WishList
    {
        public int Id { get; init; }
        [Column(TypeName = "varchar(30)")]
        public string Code { get; set; } = null!;
        public int CantItems { get; set; }
        public Guid CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public int StatusId { get; set; }
        public virtual Status? Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public List<WishListProduct> WishListProducts { get; set; } = null!;
    }
}
