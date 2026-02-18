using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class ShippingCostConfiguration
    {
        public int Id { get; init; }

        // Origen del envío
        public int? SourceWarehouseId { get; set; }
        public virtual Warehouse? SourceWarehouse { get; set; }

        public int? SourceProviderId { get; set; }
        public virtual Provider? SourceProvider { get; set; }

        public int? SourceCityId { get; set; }
        public virtual City? SourceCity { get; set; }

        // Destino (opcional - si null aplica a todos)
        public int? DestinationCityId { get; set; }
        public virtual City? DestinationCity { get; set; }

        public int? DestinationDepartmentId { get; set; }
        public virtual Department? DestinationDepartment { get; set; }

        // Producto específico (opcional - para configuración por producto)
        public int? ProductId { get; set; }
        public virtual Product? Product { get; set; }

        // Costos
        [Precision(18, 2)]
        public decimal MinCost { get; set; }

        [Precision(18, 2)]
        public decimal MaxCost { get; set; }

        [Precision(18, 2)]
        public decimal DefaultCost { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string CostType { get; set; } = null!; // "Pickup", "Delivery", "Courier"

        [Column(TypeName = "varchar(500)")]
        public string? Notes { get; set; }

        public bool IsActive { get; set; }
        public int Priority { get; set; } // Para resolver conflictos

        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModificatedBy { get; set; }
    }
}
