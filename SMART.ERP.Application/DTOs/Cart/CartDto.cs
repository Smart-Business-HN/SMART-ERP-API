using SMART.ERP.Application.DTOs.CartItem;
using SMART.ERP.Application.DTOs.EcommerceUser;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.DTOs.Cart
{
    public class CartDto
    {
        public Guid Id { get; init; }
        public Guid EcomerceUserId { get; set; }
        public EcommerceUserDto? EcommerceUser { get; set; }
        public bool IsActive { get; set; }
        public CartStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public string? PaymentLinkUrl { get; set; }
        public DateTime CreationDate { get; set; }
        public int? DestinationQuotationId { get; set; }
        public QuotationDto? DestinationQuotation { get; set; }
        public DateTime? ConvertionDate { get; set; }
        public virtual List<CartItemDto>? CartItems { get; set; }
    }
}
