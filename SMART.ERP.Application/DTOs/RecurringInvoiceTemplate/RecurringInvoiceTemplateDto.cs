using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.DTOs.InvoicePaymentType;
using SMART.ERP.Application.DTOs.Status;
using SMART.ERP.Application.DTOs.User;

namespace SMART.ERP.Application.DTOs.RecurringInvoiceTemplate
{
    public class RecurringInvoiceTemplateDto
    {
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public CustomerDto? Customer { get; set; }
        public int BranchOfficeId { get; set; }
        public BranchOfficeDto? BranchOffice { get; set; }
        public Guid UserId { get; set; }
        public UserDto? User { get; set; }
        public int InvoicePaymentTypeId { get; set; }
        public InvoicePaymentTypeDto? InvoicePaymentType { get; set; }
        public int DayOfMonth { get; set; }
        public string? Observations { get; set; }
        public string? TermsAndConditions { get; set; }
        public int StatusId { get; set; }
        public StatusDto? Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? LastGeneratedDate { get; set; }
        public DateTime? NextGenerationDate { get; set; }
        public int? ProjectId { get; set; }
        public List<RecurringInvoiceTemplateItemDto>? Items { get; set; }
    }
}
