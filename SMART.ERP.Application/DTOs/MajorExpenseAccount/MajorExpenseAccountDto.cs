using SMART.ERP.Application.DTOs.ExpenseAccount;

namespace SMART.ERP.Application.DTOs.MajorExpenseAccount
{
    public class MajorExpenseAccountDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<ExpenseAccountDto>? ExpenseAccounts { get; set; }
    }
}
