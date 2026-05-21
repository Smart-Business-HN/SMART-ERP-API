namespace SMART.ERP.Application.DTOs.PriceList
{
    public class ResolvedPricesBatchDto
    {
        public int? PriceListId { get; set; }
        public string? PriceListName { get; set; }
        public bool IsDefault { get; set; }
        public List<ResolvedBatchItemDto> Items { get; set; } = new();
    }

    public class ResolvedBatchItemDto
    {
        public int ProductId { get; set; }
        public decimal? Price { get; set; }
        public bool HasPrice { get; set; }
    }
}
