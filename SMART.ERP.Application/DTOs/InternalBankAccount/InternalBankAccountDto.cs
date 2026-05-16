using SMART.ERP.Application.DTOs.Bank;
using SMART.ERP.Domain.Enums;

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
        public InternalBankAccountType AccountType { get; set; }
        public string? CardLastFour { get; set; }
    }
}
