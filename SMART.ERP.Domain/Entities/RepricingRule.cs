using Microsoft.EntityFrameworkCore;
using SMART.ERP.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    /// <summary>
    /// Regla de re-fijación por producto (override). Si un producto no tiene fila, se usan los valores
    /// por defecto de <see cref="RepricingSettings"/>.
    /// </summary>
    public class RepricingRule
    {
        public int Id { get; init; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        public UndercutMode UndercutMode { get; set; } = UndercutMode.Percent;

        /// <summary>Porcentaje (0.01 = 1%) o monto fijo (L.) según <see cref="UndercutMode"/>.</summary>
        [Precision(18, 4)]
        public decimal UndercutValue { get; set; } = 0.01m;

        /// <summary>Margen mínimo sobre el costo: floor = CostPrice × (1 + MinMarginPercent). 0.15 = 15%.</summary>
        [Precision(9, 4)]
        public decimal MinMarginPercent { get; set; } = 0.15m;

        public PriceRoundingMode RoundingMode { get; set; } = PriceRoundingMode.None;

        /// <summary>Guarda anti-espiral: no bajar más de este % respecto al precio actual en una sola corrida. 0 = sin guarda.</summary>
        [Precision(9, 4)]
        public decimal MaxDecreasePercent { get; set; }

        /// <summary>No escribir si |target − precio actual| es menor a este umbral (ignora ruido).</summary>
        [Precision(18, 2)]
        public decimal MinChangeThreshold { get; set; }

        public bool AutoApply { get; set; } = true;
        public bool IsEnabled { get; set; } = true;

        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [MaxLength(50)]
        public string? ModificatedBy { get; set; }
    }
}
