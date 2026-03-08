using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class QuotationItemObservation
    {
        public int Id { get; set; }
        public int ProductOfferedId { get; set; }
        public virtual ProductOffered? ProductOffered { get; set; }
        public int QuotationId { get; set; }
        public virtual Quotation? Quotation { get; set; }
        [MaxLength(100)]
        public string AuthorName { get; set; } = null!;
        [MaxLength(500)]
        public string Observation { get; set; } = null!;
        public DateTime CreationDate { get; set; }
    }
}
