using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Domain.Entities
{
    public class MajorExpenseAccount
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<ExpenseAccount>? ExpenseAccounts { get; set; }
    }
}
