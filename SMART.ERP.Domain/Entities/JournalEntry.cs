using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Domain.Entities
{
    /// <summary>
    /// Asiento contable (Libro Diario). Cabecera de partida doble: la suma del debe debe ser igual
    /// a la suma del haber. Una vez contabilizado (Posted) es inmutable y solo puede reversarse.
    /// </summary>
    public class JournalEntry
    {
        public int Id { get; init; }
        [MaxLength(20)]
        public string? EntryNumber { get; set; }
        public DateTime EntryDate { get; set; }
        public int FiscalPeriodId { get; set; }
        public virtual FiscalPeriod? FiscalPeriod { get; set; }
        [MaxLength(300)]
        public string Description { get; set; } = null!;
        [MaxLength(50)]
        public string? Reference { get; set; }
        public JournalEntryStatus Status { get; set; }
        public JournalEntrySource Source { get; set; }

        // Seam para posteo automático (Fase 2): documento operativo de origen.
        [MaxLength(50)]
        public string? SourceDocumentType { get; set; }
        public int? SourceDocumentId { get; set; }

        [Precision(18, 2)]
        public decimal TotalDebit { get; set; }
        [Precision(18, 2)]
        public decimal TotalCredit { get; set; }

        public DateTime? PostedDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? PostedBy { get; set; }

        // Relaciones de reversa (auto-referenciadas)
        public int? ReversesJournalEntryId { get; set; }
        public virtual JournalEntry? ReversesJournalEntry { get; set; }
        public int? ReversedByJournalEntryId { get; set; }
        public virtual JournalEntry? ReversedByJournalEntry { get; set; }

        public virtual List<JournalEntryLine>? Lines { get; set; }

        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModifiedBy { get; set; }
    }
}
