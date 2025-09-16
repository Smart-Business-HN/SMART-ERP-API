using SMART.ERP.Application.DTOs.Cart;
using SMART.ERP.Application.DTOs.Product;

namespace SMART.ERP.Application.DTOs.CartItem
{
    public class CartItemDto
    {
        public int Id { get; init; }
        public Guid CartId { get; set; }
        public CartDto? Cart { get; set; }
        public int ProductId { get; set; }
        public ProductDto? Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Discount { get; set; } = 0;
        public decimal TotalPrice { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
