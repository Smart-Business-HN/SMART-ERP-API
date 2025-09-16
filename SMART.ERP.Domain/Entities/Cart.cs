namespace SMART.ERP.Domain.Entities
{
    public class Cart
    {
        public Guid Id { get; init; }
        public Guid EcommerceUserId { get; set; }
        public EcommerceUser? EcommerceUser { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public int? DestinationQuotationId { get; set; }
        public Quotation? DestinationQuotation { get; set; }
        public DateTime? ConvertionDate { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
    }
}
