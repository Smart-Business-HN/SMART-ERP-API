namespace SMART.ERP.Application.DTOs.Customer
{
    public class PurchaseDateCountDto
    {
        public DateOnly Date { get; set; }
        public int Count { get; set; }
        public decimal Total { get; set; }
    }
}
