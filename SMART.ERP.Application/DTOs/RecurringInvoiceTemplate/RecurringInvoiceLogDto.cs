namespace SMART.ERP.Application.DTOs.RecurringInvoiceTemplate
{
    public class RecurringInvoiceLogDto
    {
        public int Id { get; set; }
        public int RecurringInvoiceTemplateId { get; set; }
        public int? GeneratedInvoiceId { get; set; }
        public DateTime ExecutionDate { get; set; }
        public bool Succeeded { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
