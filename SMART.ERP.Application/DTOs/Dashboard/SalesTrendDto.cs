namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class SalesTrendDto
    {
        public List<SalesMonthDto> Months { get; set; } = new();
    }

    public class SalesMonthDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal TotalSales { get; set; }
        public int InvoiceCount { get; set; }
    }
}
