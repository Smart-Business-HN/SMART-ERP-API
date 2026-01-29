using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class MajorExpenseAccount
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        public List<ExpenseAccount>? ExpenseAccounts { get; set; }
    }
}
