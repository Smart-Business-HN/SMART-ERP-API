using SMART.ERP.Application.DTOs.InternalBankAccount;
using SMART.ERP.Application.DTOs.NonBilllableExpense;
using SMART.ERP.Application.DTOs.TypeOfPaymentMethod;

namespace SMART.ERP.Application.DTOs.NonBillableExpensePayment
{
    public class NonBillableExpensePaymentDto
    {
        public int Id { get; set; }
        public int NonBillableExpenseId { get; set; }
        public NonBillableExpenseDto? NonBillableExpense { get; set; }
        public int TypeOfPaymentMethodId { get; set; }
        public TypeOfPaymentMethodDto? TypeOfPaymentMethod { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int? InternalBankAccountId { get; set; }
        public InternalBankAccountDto? InternalBankAccount { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public string? Attachment { get; set; }
    }
}
