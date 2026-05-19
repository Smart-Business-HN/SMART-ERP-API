namespace SMART.ERP.Application.DTOs.Provider
{
    public class ProviderPurchaseBillLineDto
    {
        public int Id { get; set; }
        public string PurchaseBillCode { get; set; } = null!;
        public string InvoiceNumber { get; set; } = null!;
        public DateTime InvoiceDate { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public decimal Total { get; set; }
        public decimal Outstanding { get; set; }
    }
}
