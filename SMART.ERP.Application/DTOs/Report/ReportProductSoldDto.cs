namespace SMART.ERP.Application.DTOs.Report
{
    public class ReportProductSoldDto
    {
        public string Name { get; set; } = null!;
        public string SubCategory { get; set; } = null!;
        public string Category { get; set; } = null!;
        public int Quantity { get; set; } = 0;
        public decimal Total { get; set; } = 0;
    }
}
