namespace SMART.ERP.Application.DTOs.Quotation
{
    public class QuotationSnapshotDataDto
    {
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public string? QuotationCode { get; set; }
        public int BranchOfficeId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? Observations { get; set; }
        public string? TermsAndConditions { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public int StatusId { get; set; }
        public int PrefixId { get; set; }
        public decimal? Profitability { get; set; }
        public int? InvoiceDestinationId { get; set; }
        public int? ProjectId { get; set; }
        public decimal TotalShippingCost { get; set; }
        public decimal SubTotalWithoutShipping { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime InsertedDate { get; set; }
        public string? ModificatedBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        public List<ProductOfferedSnapshotDto> ProductsOffered { get; set; } = new();
    }

    public class ProductOfferedSnapshotDto
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string ProductDescription { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public int TaxId { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalLine { get; set; }
        public int? SourceWarehouseId { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal SubTotalWithoutShipping { get; set; }
        public bool IsFromVirtualStock { get; set; }
    }
}
