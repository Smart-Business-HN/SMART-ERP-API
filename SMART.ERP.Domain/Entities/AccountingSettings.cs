using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    /// <summary>
    /// Configuración global del módulo contable (fila única). Controla el posteo automático
    /// de asientos desde documentos operativos.
    /// </summary>
    public class AccountingSettings
    {
        public int Id { get; init; }
        /// <summary>Si está activo, los documentos generan asientos automáticamente al guardarse.</summary>
        public bool AutoPostingEnabled { get; set; }

        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModifiedBy { get; set; }
    }
}
