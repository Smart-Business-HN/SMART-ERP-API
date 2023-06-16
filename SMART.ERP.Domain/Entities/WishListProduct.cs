namespace SMART.ERP.Domain.Entities
{
    public class WishListProduct
    {
        public int Id { get; init; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public int WishListId { get; set; }
        public virtual WishList? WishList { get; set; }
        public int StatusId { get; set; }
        public virtual Status? Status { get; set; }
        public DateTime CreationDate { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
    }
}
