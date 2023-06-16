using SMART.ERP.Application.DTOs.Product;

namespace SMART.ERP.Application.DTOs.Report
{
    public class ClientQuoteProductDto
    {
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public BasicDetailProductDto? Product { get; set; }
    }
}
