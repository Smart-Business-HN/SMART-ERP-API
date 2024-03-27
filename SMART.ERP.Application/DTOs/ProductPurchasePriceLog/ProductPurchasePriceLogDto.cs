using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.DTOs.PurchaseBill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.DTOs.ProductPurchasePriceLog
{
    public class ProductPurchasePriceLogDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual ProductDto? Product { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int PurchaseBillOriginId { get; set; }
        public virtual PurchaseBillDto? PurchaseBillOrigin { get; set; }
        public decimal UnitsPurchased { get; set; }
        public decimal Price { get; set; }
    }
}
