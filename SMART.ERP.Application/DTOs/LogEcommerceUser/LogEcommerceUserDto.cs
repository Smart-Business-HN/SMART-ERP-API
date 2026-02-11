namespace SMART.ERP.Application.DTOs.LogEcommerceUser;

public class LogEcommerceUserDto
{
    public int Id { get; set; }
    public Guid EcommerceUserId { get; set; }
    public int ActionType { get; set; }
    public string ActionTypeName { get; set; } = null!;
    public string? Details { get; set; }
    public DateTime CreationDate { get; set; }
}
