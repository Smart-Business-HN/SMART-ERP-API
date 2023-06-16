namespace SMART.ERP.Application.DTOs.Report
{
    public class ReportClientQuoteDto
    {
        public string FullName { get; set; } = null!;
        public List<ClientQuoteProductDto> Products { get; set; } = new List<ClientQuoteProductDto>();
    }
}
