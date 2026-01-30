using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.DTOs.ExpenseAccount;
using SMART.ERP.Application.DTOs.NonBillableExpensePayment;
using SMART.ERP.Application.DTOs.Provider;
using SMART.ERP.Application.DTOs.Status;

namespace SMART.ERP.Application.DTOs.NonBilllableExpense
{
    public class NonBillableExpenseDto
    {
        public int Id { get; init; }
        public int ProviderId { get; set; }
        public virtual ProviderDto? Provider { get; set; }
        public string Description { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int ExpenseAccountId { get; set; }
        public virtual ExpenseAccountDto? ExpenseAccount { get; set; }
        public decimal Outstanding { get; set; }
        public int PrefixId { get; set; }
        public virtual PrefixDto? Prefix { get; set; }
        public int StatusId { get; set; }
        public virtual StatusDto? Status { get; set; }
        public string ExpenseCode { get; set; } = null!;
        public int? ProjectId { get; set; }
        public List<NonBillableExpensePaymentDto>? NonBillableExpensePayments { get; set; }
    }
}
