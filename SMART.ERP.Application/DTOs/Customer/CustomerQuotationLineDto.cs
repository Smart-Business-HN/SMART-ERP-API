namespace SMART.ERP.Application.DTOs.Customer
{
    public class CustomerQuotationLineDto
    {
        public int Id { get; set; }
        public string? QuotationCode { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DueDate { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public decimal Total { get; set; }
    }
}
