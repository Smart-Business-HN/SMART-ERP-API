namespace SMART.ERP.Application.DTOs.RecurringInvoiceTemplate
{
    public class RecurringInvoiceTemplateItemDto
    {
        public int Id { get; set; }
        public int RecurringInvoiceTemplateId { get; set; }
        public int? ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string ProductDescription { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public int TaxId { get; set; }
    }
}
