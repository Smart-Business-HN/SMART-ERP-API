using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class QuotationSnapshot
    {
        public int Id { get; init; }
        public int QuotationId { get; set; }
        public virtual Quotation? Quotation { get; set; }
        public int VersionNumber { get; set; }
        public string SnapshotData { get; set; } = null!;
        public string? ChangeSummary { get; set; }
        [MaxLength(50)]
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
    }
}
