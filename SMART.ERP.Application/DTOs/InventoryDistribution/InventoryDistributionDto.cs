using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.DTOs.Warehouse;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.DTOs.InventoryDistribution
{
    public class InventoryDistributionDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public  ProductDto Product { get; set; } = null!;
        public int WarehouseId { get; set; }
        public WarehouseDto Warehouse { get; set; } = null!;
        public double Quantity { get; set; }
    }
}
