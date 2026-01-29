namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class ExpensesDto
    {
        public List<string> ExpenseAccounts {  get; set; } = [];
        public List<decimal> Values { get; set; } = [];
    }
}
