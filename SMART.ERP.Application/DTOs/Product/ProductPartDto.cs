namespace SMART.ERP.Application.DTOs.Product
{
    public class ProductPartDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public decimal Quantity { get; set; }
        public bool IsActive { get; set; }
        public int ComponentCurrentStock { get; set; }
        public decimal ComponentCostPrice { get; set; }
    }
}
