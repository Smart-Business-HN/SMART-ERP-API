using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.DTOs.Product;

namespace SMART.ERP.Application.DTOs.Quotation
{
    public class ProductOfferedDto
    {
        public int Id { get; set; }
        public int QuotationId { get; set; }
        public QuotationDto? Quotation { get; set; }
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
        // Dropshipping fields
        public int? SourceWarehouseId { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal SubTotalWithoutShipping { get; set; }
        public bool IsFromVirtualStock { get; set; }
    }
}
