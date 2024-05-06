using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.DTOs.Product;

namespace SMART.ERP.Application.DTOs.Invoice
{
    public class ProductSoldDto
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public InvoiceDto? Invoice { get; set; }
        public int? ProductId { get; set; }
        public ProductDto? Product { get; set; }
        public string? ProductCode { get; set; }
        public string ProductDescription { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public int TaxId { get; set; }
        public TaxDto? Tax { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalLine { get; set; }
    }
}
