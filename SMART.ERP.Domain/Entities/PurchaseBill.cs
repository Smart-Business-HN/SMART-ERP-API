using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Domain.Entities
{
    public class PurchaseBill
    {
        public int Id { get; init; }
        public int ProviderId { get; set; }
        public Provider? Provider { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public DateTime InvoiceDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string Cai { get; set; } = null!;
        public int? PurchaseOrderOriginId { get; set; }
        public PurchaseOrder? PurchaseOrderOrigin { get; set; }
        public int StatusId { get; set; }
        public Status? Status { get; set; }
        public decimal Exempt {  get; set; }
        public decimal Exonerated { get; set; }
        public decimal TaxedAt15Percent { get; set; }
        public decimal TaxedAt18Percent { get; set; }
        public decimal Taxes15Percent {  get; set; }
        public decimal Taxes18Percent { get; set; }
        public decimal Total { get; set; }
        public int? InventoryInputDestinationId { get; set; }
    }
}
