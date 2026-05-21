namespace SMART.ERP.Application.DTOs.Customer
{
    public class CustomerInvoiceLineDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public decimal Total { get; set; }
        public decimal Outstanding { get; set; }
    }
}
