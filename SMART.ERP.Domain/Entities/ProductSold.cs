namespace SMART.ERP.Domain.Entities
{
    public class ProductSold
    {
        public int Id { get; init; }
        public int InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; }
        public int? ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public string? ProductCode { get; set; }
        public string ProductDescription { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public int TaxId { get; set; }
        public virtual Tax? Tax { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalLine { get; set; }
    }
}
