namespace SMART.ERP.Application.DTOs.Report
{
    public class ClientByProductSoldDto
    {
        public string CustomerName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}
