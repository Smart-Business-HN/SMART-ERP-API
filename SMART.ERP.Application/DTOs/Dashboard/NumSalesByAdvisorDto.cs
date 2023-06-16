namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class NumSalesByAdvisorDto
    {
        public string FullName { get; set; } = null!;
        public decimal? Total { get; set; }
        public int NumSales { get; set; }
    }
}
