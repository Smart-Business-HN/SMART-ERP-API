using SMART.ERP.Application.DTOs.NonBilllableExpense;
using SMART.ERP.Application.DTOs.Provider;
using SMART.ERP.Application.DTOs.PurchaseBill;

namespace SMART.ERP.Application.DTOs.AccountsPayable;

public class AccountsPayableDto
{
    public int ProviderId { get; set; }
    public ProviderDto? Provider { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalOutstanding { get; set; }
    public List<PurchaseBillDto>? PurchaseBills { get; set; }
    public List<NonBillableExpenseDto>? NonBillableExpenses { get; set; }
}