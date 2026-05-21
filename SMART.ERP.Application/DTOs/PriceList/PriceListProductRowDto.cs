namespace SMART.ERP.Application.DTOs.PriceList
{
    public class PriceListProductRowDto
    {
        public int ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public decimal CostPrice { get; set; }
        public decimal RecomendedSalePrice { get; set; }
        public decimal? Price { get; set; }
        public bool HasCustomPrice { get; set; }
        public int? PriceListItemId { get; set; }
    }
}
