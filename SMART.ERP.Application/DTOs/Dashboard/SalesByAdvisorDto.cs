namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class SalesByAdvisorDto
    {
        public string FullName { get; set; } = null!;
        public List<decimal> Data { get; set; } = new List<decimal> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    }
}
