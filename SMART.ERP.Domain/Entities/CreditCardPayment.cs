using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    /// <summary>
    /// Pago de una tarjeta de crédito desde una cuenta bancaria interna. Genera asiento contable
    /// Dr (LedgerAccount de la TC) / Cr (LedgerAccount del banco origen). Ambas cuentas se modelan
    /// como InternalBankAccount diferenciadas por AccountType (CreditCard vs Checking/Savings).
    /// </summary>
    public class CreditCardPayment
    {
        public int Id { get; init; }
        [MaxLength(12)]
        public string Code { get; set; } = null!;
        public int CreditCardInternalBankAccountId { get; set; }
        public virtual InternalBankAccount? CreditCardInternalBankAccount { get; set; }
        public int SourceInternalBankAccountId { get; set; }
        public virtual InternalBankAccount? SourceInternalBankAccount { get; set; }
        public DateTime Date { get; set; }
        [Precision(18, 2)]
        public decimal Amount { get; set; }
        [MaxLength(100)]
        public string? Reference { get; set; }
        [MaxLength(500)]
        public string? Notes { get; set; }
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModifiedBy { get; set; }
    }
}
