namespace SMART.ERP.Application.DTOs.Quotation
{
    public class ComboComponentPreviewDto
    {
        public string Name { get; set; } = null!;
        public decimal Quantity { get; set; }
        public string? UnitOfMeasurement { get; set; }
        public int DisplayOrder { get; set; }
    }
}
