using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Domain.Entities
{
    public class InternalBankAccount
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public int BankId { get; set; }
        public Bank? Bank { get; set; }
        public string? AccountNumber { get; set; }
        public decimal CurrentAmount { get; set; }
    }
}
