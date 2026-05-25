using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.DTOs.AccountingConfig
{
    public class AccountingSettingsDto
    {
        public bool AutoPostingEnabled { get; set; }
    }

    public class AccountingMappingDto
    {
        public AccountMappingKey Key { get; set; }
        public string KeyName { get; set; } = null!;
        public int? LedgerAccountId { get; set; }
        public string? LedgerAccountCode { get; set; }
        public string? LedgerAccountName { get; set; }
    }

    public class BankAccountMappingDto
    {
        public int InternalBankAccountId { get; set; }
        public string Name { get; set; } = null!;
        public int? LedgerAccountId { get; set; }
        public string? LedgerAccountCode { get; set; }
        public string? LedgerAccountName { get; set; }
    }

    public class ExpenseAccountMappingDto
    {
        public int ExpenseAccountId { get; set; }
        public string Name { get; set; } = null!;
        public int? LedgerAccountId { get; set; }
        public string? LedgerAccountCode { get; set; }
        public string? LedgerAccountName { get; set; }
    }
}
