namespace SMART.ERP.Application.DTOs.Auth
{
    public class SesionCustomerDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? Avatar { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpirationDate { get; set; }
        public int? WishListId { get; set; }
    }
}
