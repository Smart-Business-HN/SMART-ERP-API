using SMART.ERP.Application.DTOs.InternalBankAccount;

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
