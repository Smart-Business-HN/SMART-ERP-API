using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class NonBillableExpensePayment
    {
        public int Id { get; init; }
        public int NonBillableExpenseId { get; set; }
        public NonBillableExpense? NonBillableExpense { get; set; }
        public int TypeOfPaymentMethodId { get; set; }
        public TypeOfPaymentMethod? TypeOfPaymentMethod { get; set; }
        public DateTime Date { get; set; }
        [Precision(18, 2)]
        public decimal Amount { get; set; }
        public int? InternalBankAccountId { get; set; }
        public InternalBankAccount? InternalBankAccount { get; set; }
        public DateTime CreationDate { get; set; }
        [MaxLength(50)]
        public string CreatedBy { get; set; } = null!;
        public string? Attachment { get; set; }
    }
}
