using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Domain.Entities
{
    public class ProductEntry
    {
        public int Id { get; init; }
        public int InventoryInputId { get; set; }
        public virtual InventoryInput? InventoryInput { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public double Quantity { get; set; }
        public double UnitProductPrice { get; set; }
        public double Total { get; set; }
    }
}
