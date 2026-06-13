using Microsoft.EntityFrameworkCore;
using SMART.ERP.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    /// <summary>
    /// Configuración global del módulo de re-fijación de precios (fila única). Provee los toggles y los
    /// valores por defecto de la regla cuando un producto no tiene <see cref="RepricingRule"/> propia.
    /// </summary>
    public class RepricingSettings
    {
        public int Id { get; init; }

        /// <summary>Si está apagado, el job no scrapea ni reprecia.</summary>
        public bool MonitoringEnabled { get; set; }

        /// <summary>Auto-aplicar precios globalmente (default para productos sin regla propia).</summary>
        public bool GlobalAutoApply { get; set; }

        public UndercutMode DefaultUndercutMode { get; set; } = UndercutMode.Percent;
        [Precision(18, 4)]
        public decimal DefaultUndercutValue { get; set; } = 0.01m;
        [Precision(9, 4)]
        public decimal DefaultMinMarginPercent { get; set; } = 0.15m;
        public PriceRoundingMode DefaultRoundingMode { get; set; } = PriceRoundingMode.None;
        [Precision(9, 4)]
        public decimal DefaultMaxDecreasePercent { get; set; }
        [Precision(18, 2)]
        public decimal DefaultMinChangeThreshold { get; set; }

        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModifiedBy { get; set; }
    }
}
