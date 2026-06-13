using Microsoft.EntityFrameworkCore;
using SMART.ERP.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    /// <summary>
    /// Auditoría de cada evaluación de re-fijación: qué competidor fijó el mínimo, precio anterior,
    /// precio propuesto, si se aplicó y por qué.
    /// </summary>
    public class PriceChangeLog
    {
        public int Id { get; init; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        /// <summary>Fuente de competencia que determinó el precio mínimo de referencia (si hubo).</summary>
        public int? CompetitorSourceIdMin { get; set; }

        [Precision(18, 2)]
        public decimal? MinCompetitorPrice { get; set; }
        [Precision(18, 2)]
        public decimal OldPrice { get; set; }
        [Precision(18, 2)]
        public decimal ProposedPrice { get; set; }
        [Precision(18, 2)]
        public decimal? AppliedPrice { get; set; }

        public bool FloorHit { get; set; }
        public bool Applied { get; set; }
        public PriceChangeStatus Status { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string? Reason { get; set; }

        public DateTime CreatedUtc { get; set; }
        public DateTime? AppliedUtc { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? AppliedBy { get; set; }
    }
}
