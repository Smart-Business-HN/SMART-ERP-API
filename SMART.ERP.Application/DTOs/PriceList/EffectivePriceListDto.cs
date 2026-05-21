namespace SMART.ERP.Application.DTOs.PriceList
{
    public class EffectivePriceListDto
    {
        public int? PriceListId { get; set; }
        public string? PriceListName { get; set; }
        /// <summary>"customer" (override propio), "customerType" (heredada del tipo), "default" o "none".</summary>
        public string Source { get; set; } = "none";
    }
}
