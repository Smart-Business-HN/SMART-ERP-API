namespace SMART.ERP.Application.DTOs.Quotation
{
    public class QuotationItemObservationDto
    {
        public int Id { get; set; }
        public int ProductOfferedId { get; set; }
        public int QuotationId { get; set; }
        public string AuthorName { get; set; } = null!;
        public string Observation { get; set; } = null!;
        public DateTime CreationDate { get; set; }
    }
}
