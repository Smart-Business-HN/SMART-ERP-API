using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Domain.Entities
{
    /// <summary>
    /// Ejercicio fiscal (año contable). Agrupa 12 períodos mensuales. Al cerrarse se genera
    /// el asiento de cierre que traslada las cuentas de resultado al patrimonio.
    /// </summary>
    public class FiscalYear
    {
        public int Id { get; init; }
        public int Year { get; set; }
        [MaxLength(60)]
        public string Name { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public FiscalPeriodStatus Status { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? ClosedDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ClosedBy { get; set; }
        public int? ClosingJournalEntryId { get; set; }
        public virtual JournalEntry? ClosingJournalEntry { get; set; }
        public virtual List<FiscalPeriod>? Periods { get; set; }

        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModifiedBy { get; set; }
    }
}
