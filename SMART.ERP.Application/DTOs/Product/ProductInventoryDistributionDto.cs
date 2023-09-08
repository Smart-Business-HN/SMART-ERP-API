using SMART.ERP.Application.DTOs.Warehouse;

namespace SMART.ERP.Application.DTOs.Product
{
    public class ProductInventoryDistributionDto
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public WarehouseDto Warehouse { get; set; } = null!;
        public double Quantity { get; set; }
    }
}
