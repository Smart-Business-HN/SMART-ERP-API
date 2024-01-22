using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Domain.Entities
{
    public class ProductToPurchase
    {
        public int Id { get; init; }
        public int PurchaseOrderId { get; set; }
        public virtual PurchaseOrder? PurchaseOrder { get; set; }
        public int? ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public int TaxId { get; set; }
        public virtual Tax? Tax { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalLine { get; set; }
    }
}
