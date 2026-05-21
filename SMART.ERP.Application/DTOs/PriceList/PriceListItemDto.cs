namespace SMART.ERP.Application.DTOs.PriceList
{
    public class PriceListItemDto
    {
        public int Id { get; set; }
        public int PriceListId { get; set; }
        public int ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
        public decimal MarginPct { get; set; }
    }
}
