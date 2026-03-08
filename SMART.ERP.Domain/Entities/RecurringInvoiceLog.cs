using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class RecurringInvoiceLog
    {
        public int Id { get; init; }
        public int RecurringInvoiceTemplateId { get; set; }
        public virtual RecurringInvoiceTemplate? RecurringInvoiceTemplate { get; set; }
        public int? GeneratedInvoiceId { get; set; }
        public virtual Invoice? GeneratedInvoice { get; set; }
        public DateTime ExecutionDate { get; set; }
        public bool Succeeded { get; set; }
        [MaxLength(500)]
        public string? ErrorMessage { get; set; }
    }
}
