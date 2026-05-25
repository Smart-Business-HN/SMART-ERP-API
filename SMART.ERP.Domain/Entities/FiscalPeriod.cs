using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Domain.Entities
{
    /// <summary>
    /// Período contable mensual (1..12) dentro de un ejercicio fiscal. El estado controla
    /// el bloqueo de contabilización por fecha del asiento.
    /// </summary>
    public class FiscalPeriod
    {
        public int Id { get; init; }
        public int FiscalYearId { get; set; }
        public virtual FiscalYear? FiscalYear { get; set; }
        public int PeriodNumber { get; set; }
        [MaxLength(40)]
        public string Name { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public FiscalPeriodStatus Status { get; set; }
        public DateTime? ClosedDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ClosedBy { get; set; }

        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModifiedBy { get; set; }
    }
}
