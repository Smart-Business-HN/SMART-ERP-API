using SMART.ERP.Application.DTOs.Product;

namespace SMART.ERP.Application.DTOs.Opportunity
{
    public class QuoteProductDto
    {
        public int Id { get; set; }
        public int OpportunityId { get; set; }
        public int ProductId { get; set; }
        public decimal SalePrice { get; set; }
        public int Quantity { get; set; }
        public BasicDetailProductDto? Product { get; set; }
    }
}
