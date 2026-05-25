using System.ComponentModel.DataAnnotations.Schema;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Domain.Entities
{
    /// <summary>
    /// Mapeo de una cuenta de sistema (AccountMappingKey) a una cuenta imputable del catálogo,
    /// usado por el posteo automático.
    /// </summary>
    public class AccountingMapping
    {
        public int Id { get; init; }
        public AccountMappingKey Key { get; set; }
        public int? LedgerAccountId { get; set; }
        public virtual LedgerAccount? LedgerAccount { get; set; }

        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModifiedBy { get; set; }
    }
}
