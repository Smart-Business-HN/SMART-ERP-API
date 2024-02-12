using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.DTOs.Warehouse;

namespace SMART.ERP.Application.DTOs.InventoryDistribution
{
    public class InventoryDistributionDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public ProductDto? Product { get; set; }
        public int WarehouseId { get; set; }
        public WarehouseDto? Warehouse { get; set; }
        public decimal Quantity { get; set; }
    }
}
