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
        public decimal UnitPrice { get; set; }
        public decimal? Discount { get; set; } = 0;
        public decimal TotalPrice { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
