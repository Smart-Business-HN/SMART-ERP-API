using SMART.ERP.Application.DTOs.InternalBankAccount;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.DTOs.Bank
{
    public class BankDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool ItIsNationalBank { get; set; }
        public List<InternalBankAccountDto>? InternalBankAccounts { get; set; }
    }
}
