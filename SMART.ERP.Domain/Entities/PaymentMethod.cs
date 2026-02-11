using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities;

public class PaymentMethod
{
    public int Id { get; init; }
    public Guid EcommerceUserId { get; set; }
    public virtual EcommerceUser? EcommerceUser { get; set; }
    [MaxLength(50)]
    public string Alias { get; set; } = null!;
    [MaxLength(100)]
    public string CardholderName { get; set; } = null!;
    [MaxLength(4)]
    public string Last4Digits { get; set; } = null!;
    [MaxLength(500)]
    public string EncryptedCardNumber { get; set; } = null!;
    public int ExpirationMonth { get; set; }
    public int ExpirationYear { get; set; }
    [MaxLength(20)]
    public string CardBrand { get; set; } = null!;
    public DateTime CreationDate { get; set; }
    public bool IsActive { get; set; }
}
