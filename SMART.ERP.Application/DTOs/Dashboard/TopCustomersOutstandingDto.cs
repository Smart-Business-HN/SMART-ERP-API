namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class TopCustomersOutstandingDto
    {
        public List<CustomerOutstandingDto> Customers { get; set; } = new();
    }

    public class CustomerOutstandingDto
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalOutstanding { get; set; }
        public int InvoiceCount { get; set; }
        public DateTime? OldestInvoiceDate { get; set; }
    }
}
