namespace SMART.ERP.Application.DTOs.PaymentMethod;

public class PaymentMethodDto
{
    public int Id { get; set; }
    public Guid EcommerceUserId { get; set; }
    public string Alias { get; set; } = null!;
    public string CardholderName { get; set; } = null!;
    public string Last4Digits { get; set; } = null!;
    public int ExpirationMonth { get; set; }
    public int ExpirationYear { get; set; }
    public string CardBrand { get; set; } = null!;
    public DateTime CreationDate { get; set; }
    public bool IsActive { get; set; }
}
