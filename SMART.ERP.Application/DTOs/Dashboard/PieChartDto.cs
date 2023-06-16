namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class PieChartDto
    {
        public List<decimal>? series { get; set; }
        public List<string>? labels { get; set; }
    }
}
