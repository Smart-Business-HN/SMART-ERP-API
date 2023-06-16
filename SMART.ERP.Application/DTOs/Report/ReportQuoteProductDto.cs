namespace SMART.ERP.Application.DTOs.Report
{
    public class ReportQuoteProductDto
    {
        public string Name { get; set; } = null!;
        public string SubCategory { get; set; } = null!;
        public string Category { get; set; } = null!;
        public int TotalNum { get; set; } = 0;
        public decimal Total { get; set; } = 0;
        public decimal LossTotal { get; set; } = 0;
        public decimal WinTotal { get; set; } = 0;
    }
}
