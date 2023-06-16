using SMART.ERP.Application.DTOs.Product;

namespace SMART.ERP.Application.DTOs.ProspectQuoteProduct
{
    public class ProspectQuoteProductDto
    {
        public int Id { get; init; }
        public int ProductId { get; set; }
        public ProductDto? Product { get; set; }
    }
}
