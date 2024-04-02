using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Domain.Entities
{   
    public class ProductPurchasePriceLog
    {
        public int Id { get; init; }
        public int ProductId { get; set;}
        public virtual Product? Product { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int PurchaseBillOriginId { get; set; }
        public virtual PurchaseBill? PurchaseBillOrigin { get; set; }
        public decimal UnitsPurchased { get; set; }
        public decimal Price { get; set; }
    }
}
