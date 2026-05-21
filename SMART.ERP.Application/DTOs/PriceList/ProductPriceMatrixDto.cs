namespace SMART.ERP.Application.DTOs.PriceList
{
    public class ProductPriceMatrixDto
    {
        public int ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public decimal CostPrice { get; set; }
        public List<ProductPriceMatrixEntryDto> PriceLists { get; set; } = new();
    }

    public class ProductPriceMatrixEntryDto
    {
        public int PriceListId { get; set; }
        public string PriceListName { get; set; } = null!;
        public bool IsDefault { get; set; }
        public decimal? Price { get; set; }
        public int? PriceListItemId { get; set; }
    }
}
