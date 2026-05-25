using Microsoft.EntityFrameworkCore;
using SMART.ERP.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class InternalBankAccount
    {
        public int Id { get; init; }
        [MaxLength(50)]
        public string Name { get; set; } = null!;
        public int BankId { get; set; }
        public Bank? Bank { get; set; }
        [MaxLength(50)]
        public string? AccountNumber { get; set; }
        [Precision(18, 2)]
        public decimal CurrentAmount { get; set; }
        public InternalBankAccountType AccountType { get; set; } = InternalBankAccountType.Checking;
        [MaxLength(4)]
        public string? CardLastFour { get; set; }
        // Mapeo contable 1:1 hacia la cuenta del catálogo (para posteo automático).
        public int? LedgerAccountId { get; set; }
        public virtual LedgerAccount? LedgerAccount { get; set; }
    }
}
