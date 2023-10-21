using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.DTOs.Invoice;
using SMART.ERP.Application.DTOs.Status;
using SMART.ERP.Application.DTOs.User;

namespace SMART.ERP.Application.DTOs.Quotation
{
    public class QuotationDto
    {
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public string? QuotationCode { get; set; }
        public CustomerDto? Customer { get; set; }
        public int BranchOfficeId { get; set; }
        public BranchOfficeDto? BranchOffice { get; set; }
        public Guid UserId { get; set; }
        public UserDto? User { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? Observations { get; set; }
        public string? TermsAndConditions { get; set; }
        public List<ProductOfferedDto>? ProductsOffered { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public int StatusId { get; set; }
        public StatusDto? Status { get; set; }
        public int PrefixId { get; set; }
        public PrefixDto? Prefix { get; set; }
        public decimal? Profitability { get; set; }
        public int? InvoiceDestinationId { get; set; }
        public InvoiceDto? InvoiceDestination { get; set; }
    }
}
