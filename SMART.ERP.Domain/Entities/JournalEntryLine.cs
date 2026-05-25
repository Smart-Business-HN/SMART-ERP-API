using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SMART.ERP.Domain.Entities
{
    /// <summary>
    /// Partida (línea) de un asiento contable. Cada línea afecta una cuenta imputable
    /// del catálogo en el debe (Debit) o el haber (Credit); solo uno de los dos es distinto de cero.
    /// </summary>
    public class JournalEntryLine
    {
        public int Id { get; init; }
        public int JournalEntryId { get; set; }
        public virtual JournalEntry? JournalEntry { get; set; }
        public int LedgerAccountId { get; set; }
        public virtual LedgerAccount? LedgerAccount { get; set; }
        public int LineNumber { get; set; }
        [Precision(18, 2)]
        public decimal Debit { get; set; }
        [Precision(18, 2)]
        public decimal Credit { get; set; }
        [MaxLength(300)]
        public string? Description { get; set; }

        // Dimensiones del asiento (NIIF para PYMES — Honduras)
        // Centro de Costo / Línea de Negocio (obligatorio en Ingreso, Costo, Gasto Operativo).
        public int? CostCenterId { get; set; }
        public virtual CostCenter? CostCenter { get; set; }
        // Tercero: cliente o proveedor (obligatorio en CxC, CxP, retenciones, anticipos).
        public Guid? CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public int? ProviderId { get; set; }
        public virtual Provider? Provider { get; set; }
        // Proyecto: dimensión opcional (proyectos de cableado que cruzan meses, p.ej.)
        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }
    }
}
