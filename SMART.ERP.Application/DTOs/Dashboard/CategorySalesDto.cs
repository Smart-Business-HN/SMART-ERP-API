namespace SMART.ERP.Application.DTOs.Dashboard;

public class CategorySalesDto
{
    public List<string> Categories { get; set; } = [];
    public List<decimal> Values { get; set; } = [];
}