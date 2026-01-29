using Microsoft.EntityFrameworkCore;
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
    }
}
