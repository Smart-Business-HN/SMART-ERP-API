using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class QuotationComment
    {
        public int Id { get; set; }
        public int QuotationId { get; set; }
        public virtual Quotation? Quotation { get; set; }
        [MaxLength(100)]
        public string AuthorName { get; set; } = null!;
        [MaxLength(150)]
        public string? AuthorEmail { get; set; }
        [Column(TypeName = "varchar(600)")]
        public string Message { get; set; } = null!;
        public bool IsFromClient { get; set; }
        public Guid? UserId { get; set; }
        public virtual User? User { get; set; }
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModificatedBy { get; set; }
    }
}
