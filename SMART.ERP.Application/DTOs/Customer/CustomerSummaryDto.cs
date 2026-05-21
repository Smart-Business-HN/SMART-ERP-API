namespace SMART.ERP.Application.DTOs.Customer
{
    public class CustomerSummaryDto
    {
        public CustomerDto Customer { get; set; } = null!;
        public decimal TotalInvoiced { get; set; }
        public decimal OutstandingBalance { get; set; }
        public decimal TotalPayments { get; set; }
        public int InvoiceCount { get; set; }
        public decimal AveragePurchase { get; set; }
        public List<TopPurchasedProductDto> TopProducts { get; set; } = new();
        public List<PurchaseDateCountDto> PurchaseActivity { get; set; } = new();
    }
}
