namespace SMART.ERP.Application.DTOs.Quotation
{
    public class QuotationCommentDto
    {
        public int Id { get; set; }
        public int QuotationId { get; set; }
        public string AuthorName { get; set; } = null!;
        public string? AuthorEmail { get; set; }
        public string Message { get; set; } = null!;
        public bool IsFromClient { get; set; }
        public Guid? UserId { get; set; }
        public string? UserFullName { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
