namespace SMART.ERP.Application.DTOs.ProductEntry
{
    public class CreateProductEntryDto
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitProductPrice { get; set; }
        public decimal Total { get; set; }
    }
}
