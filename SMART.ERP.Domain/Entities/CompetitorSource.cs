using Microsoft.EntityFrameworkCore;
using SMART.ERP.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    /// <summary>
    /// Mapea un producto propio a la página de un competidor (Sycom, Acosa, …) y guarda el último
    /// precio observado. Una fila por (Producto, competidor).
    /// </summary>
    public class CompetitorSource
    {
        public int Id { get; init; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string CompetitorName { get; set; } = null!;

        [Column(TypeName = "varchar(max)")]
        public string ProductUrl { get; set; } = null!;

        public ParseStrategy ParseStrategy { get; set; } = ParseStrategy.Manual;

        /// <summary>Selector CSS (o ruta JSON-LD) para extraer el precio. Null cuando la estrategia es Manual.</summary>
        [Column(TypeName = "varchar(300)")]
        public string? PriceSelector { get; set; }

        public bool IsEnabled { get; set; } = true;

        public CompetitorTaxBasis TaxBasis { get; set; } = CompetitorTaxBasis.IncludesIsv15;

        [Column(TypeName = "varchar(3)")]
        public string Currency { get; set; } = "HNL";

        public DateTime? LastCheckedUtc { get; set; }

        [Precision(18, 2)]
        public decimal? LastObservedPrice { get; set; }

        public bool? LastObservedInStock { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string? LastError { get; set; }

        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [MaxLength(50)]
        public string? ModificatedBy { get; set; }
    }
}
