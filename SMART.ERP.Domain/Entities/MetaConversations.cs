using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class MetaConversations
    {
        [Column(TypeName = "varchar(30)")]
        public string Phone { get; set; } = null!;
        public int Expiration { get; set; }
        public bool ExpectingInfo { get; set; }
        public int ProductId { get; set; }
    }
}
