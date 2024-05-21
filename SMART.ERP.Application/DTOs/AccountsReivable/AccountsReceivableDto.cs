using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.DTOs.Invoice;

namespace SMART.ERP.Application.DTOs.AccountsReivable
{
    public class AccountsReceivableDto
    {
        public Guid CustomerId { get; set; }
        public CustomerDto? Customer { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal OverdueAmount { get; set; }
        public int TotalInvoices { get; set; }
        public List<InvoiceDto>? Invoices { get; set; }
    }
}
