namespace SMART.ERP.Application.DTOs.Quotation
{
    public class ProductOfferedPreviewDto
    {
        public int Id { get; set; }
        public string? ProductCode { get; set; }
        public string ProductDescription { get; set; } = null!;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int TaxId { get; set; }
        public decimal TaxRate { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalLine { get; set; }
        public List<QuotationItemObservationDto>? Observations { get; set; }
        public bool IsCombo { get; set; }
        public List<ComboComponentPreviewDto>? Components { get; set; }
    }
}
