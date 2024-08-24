namespace SMART.ERP.Application.DTOs.Discount;

public class DiscountDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Rate { get; set; }
}