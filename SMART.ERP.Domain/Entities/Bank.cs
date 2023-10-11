using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Domain.Entities
{
    public class Bank
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool ItIsNationalBank { get; set; }
        public List<InternalBankAccount>? InternalBankAccounts { get; set; }
    }
}
