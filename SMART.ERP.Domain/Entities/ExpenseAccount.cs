using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Domain.Entities
{
    public class ExpenseAccount
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;
        public int MajorExpenseAccountId { get; set; }
        public MajorExpenseAccount? MajorExpenseAccount { get; set; }
    }
}
