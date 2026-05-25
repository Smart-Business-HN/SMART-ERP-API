using SMART.ERP.Application.DTOs.Company;

namespace SMART.ERP.Application.DTOs.Quotation
{
    public class ProductToOfferdDto
    {
        public int? ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductDescription { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal RecomendedSalePrice { get; set; }
        public int TaxId { get; set; }
        public TaxDto? Tax { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalLine { get; set; }
    }
}
