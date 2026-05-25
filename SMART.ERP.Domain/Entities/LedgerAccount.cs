using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Domain.Entities
{
    /// <summary>
    /// Cuenta del Catálogo de Cuentas (Plan Contable) jerárquico, conforme a NIIF para PYMES /
    /// catálogo hondureño. Solo las cuentas hoja (IsPostable) admiten movimientos en los asientos.
    /// </summary>
    public class LedgerAccount
    {
        public int Id { get; init; }
        [MaxLength(20)]
        public string Code { get; set; } = null!;
        [MaxLength(150)]
        public string Name { get; set; } = null!;
        public AccountType AccountType { get; set; }
        public AccountLevel Level { get; set; }
        public NormalBalanceSide NormalBalanceSide { get; set; }

        // Jerarquía auto-referenciada (padre/hijos)
        public int? ParentId { get; set; }
        public virtual LedgerAccount? Parent { get; set; }
        public virtual List<LedgerAccount>? Children { get; set; }

        public bool IsPostable { get; set; }
        public bool IsActive { get; set; }
        /// <summary>Cuenta sembrada por el sistema (catálogo por defecto / cuenta de resultados). No se elimina.</summary>
        public bool IsSystem { get; set; }
        /// <summary>Si las líneas de asiento sobre esta cuenta deben llevar un Centro de Costo asignado.</summary>
        public bool RequiresCostCenter { get; set; }
        /// <summary>Si las líneas de asiento sobre esta cuenta deben llevar un Tercero (Cliente o Proveedor).</summary>
        public bool RequiresTercero { get; set; }
        /// <summary>Estado financiero al que pertenece la cuenta: BG (Balance General), ER (Estado de Resultados) o Memorando (cuentas de orden).</summary>
        [MaxLength(20)]
        public string? FinancialStatement { get; set; }
        [MaxLength(300)]
        public string? Description { get; set; }

        // Mapeo opcional con las estructuras legadas (coexistencia)
        public int? ExpenseAccountId { get; set; }
        public virtual ExpenseAccount? ExpenseAccount { get; set; }
        public int? IncomeAccountId { get; set; }
        public virtual IncomeAccount? IncomeAccount { get; set; }

        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModifiedBy { get; set; }
    }
}
