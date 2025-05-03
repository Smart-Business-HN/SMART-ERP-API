
namespace SMART.ERP.Domain.Entities
{
    public class NonBillableExpense
    {
        public int Id { get; init; }
        public int ProviderId { get; set; }
        public virtual Provider? Provider { get; set; }
        public string Description { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int ExpenseAccountId { get; set; }
        public virtual ExpenseAccount? ExpenseAccount { get; set; }
        public decimal Outstanding { get; set; }
        public int PrefixId { get; set; }
        public virtual Prefix? Prefix { get; set; }
        public int StatusId { get; set; }
        public virtual Status? Status { get; set; }
        public string ExpenseCode { get; set; } = null!;
        public virtual List<NonBillableExpensePayment>? NonBillableExpensePayments { get; set; }
    }
}
