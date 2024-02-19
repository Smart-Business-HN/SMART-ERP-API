using SMART.ERP.Application.DTOs.MajorExpenseAccount;

namespace SMART.ERP.Application.DTOs.ExpenseAccount
{
    public class ExpenseAccountDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;
        public int MajorExpenseAccountId { get; set; }
        public MajorExpenseAccountDto? MajorExpenseAccount { get; set; }
    }
}
