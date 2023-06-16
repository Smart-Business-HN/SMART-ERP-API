using SMART.ERP.Application.DTOs.Customer;

namespace SMART.ERP.Application.DTOs.Report
{
    public class ClientByQuoteProductDto
    {
        public string Code { get; set; } = null!;
        public string SalesAdvisor { get; set; } = null!;
        public CustomerDto Customer { get; set; } = null!;
        public string OpportunityStep { get; set; } = null!;
    }
}
