using SMART.ERP.Application.DTOs.Bank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.DTOs.InternalBankAccount
{
    public class InternalBankAccountDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int BankId { get; set; }
        public virtual BankDto? Bank { get; set; }
        public string? AccountNumber { get; set; }
        public decimal CurrentAmount { get; set; }
    }
}
