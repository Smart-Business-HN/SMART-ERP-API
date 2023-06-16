using SMART.ERP.Application.DTOs.Product;

namespace SMART.ERP.Application.DTOs.SaleOrder
{
    public class SaleOrderProductDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public BasicDetailProductDto? Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
