using Microsoft.EntityFrameworkCore;

namespace SMART.ERP.Domain.Entities
{
    public class CartItem
    {
        public int Id { get; init; }
        public Guid CartId { get; set; }
        public Cart? Cart { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }
        [Precision(18, 2)]
        public decimal? Discount { get; set; } = 0;
        [Precision(18, 2)]
        public decimal TotalPrice { get; set; }
        public DateTime CreationDate { get; set; }
        public string? ProductDescription { get; set; }
    }
}
