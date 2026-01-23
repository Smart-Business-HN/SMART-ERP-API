namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class MonthlyCashFlowDto
    {
        public List<CashFlowMonthDto> Months { get; set; } = new();
    }

    public class CashFlowMonthDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal Inflows { get; set; }
        public decimal Outflows { get; set; }
        public decimal NetCashFlow { get; set; }
    }
}
