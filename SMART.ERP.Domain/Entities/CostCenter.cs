using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    /// <summary>
    /// Centro de Costo / Línea de Negocio. Dimensión separada que se etiqueta en cada línea de
    /// asiento (en cuentas de Ingreso, Costo y Gasto Operativo). Reemplaza la antigua práctica
    /// de proliferar cuentas/auxiliares por línea de negocio.
    /// </summary>
    public class CostCenter
    {
        public int Id { get; init; }
        [MaxLength(10)]
        public string Code { get; set; } = null!;
        [MaxLength(120)]
        public string Name { get; set; } = null!;
        [MaxLength(400)]
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModifiedBy { get; set; }
    }
}
