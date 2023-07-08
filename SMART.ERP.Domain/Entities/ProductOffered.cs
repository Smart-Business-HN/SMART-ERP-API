namespace SMART.ERP.Domain.Entities
{
    public class ProductOffered
    {
        public int Id { get; init; }
        public int QuotationId { get; set; }
        public virtual Quotation? Quotation { get; set; }
        public int? ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public int TaxId { get; set; }
        public virtual Tax? Tax { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalLine { get; set; }
    }
}
