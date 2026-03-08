namespace SMART.ERP.Application.DTOs.Quotation
{
    public class QuotationSnapshotDto
    {
        public int Id { get; set; }
        public int QuotationId { get; set; }
        public int VersionNumber { get; set; }
        public string? ChangeSummary { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
    }

    public class QuotationSnapshotDetailDto : QuotationSnapshotDto
    {
        public QuotationSnapshotDataDto? SnapshotData { get; set; }
    }
}
