using Microsoft.EntityFrameworkCore;

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
        [Precision(18, 2)]
        public decimal UnitsPurchased { get; set; }
        [Precision(18, 2)]
        public decimal Price { get; set; }
    }
}
