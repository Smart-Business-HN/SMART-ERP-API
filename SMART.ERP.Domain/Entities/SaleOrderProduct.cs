using Microsoft.EntityFrameworkCore;

namespace SMART.ERP.Domain.Entities
{
    public class SaleOrderProduct
    {
        public int Id { get; init; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public int Quantity { get; set; }
        [Precision(18, 2)]
        public decimal Price { get; set; }
        public int SaleOrderId { get; set; }
        public virtual SaleOrder? SaleOrder { get; set; }
    }
}
