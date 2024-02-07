using SMART.ERP.Application.DTOs.Provider;
using SMART.ERP.Application.DTOs.PurchaseOrder;
using SMART.ERP.Application.DTOs.Status;

namespace SMART.ERP.Application.DTOs.PurchaseBill
{
    public class PurchaseBillDto
    {
        public int Id { get; init; }
        public int ProviderId { get; set; }
        public ProviderDto? Provider { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public DateTime InvoiceDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string Cai { get; set; } = null!;
        public int? PurchaseOrderOriginId { get; set; }
        public PurchaseOrderDto? PurchaseOrderOrigin { get; set; }
        public int StatusId { get; set; }
        public StatusDto? Status { get; set; }
        public decimal Exempt { get; set; }
        public decimal Exonerated { get; set; }
        public decimal TaxedAt15Percent { get; set; }
        public decimal TaxedAt18Percent { get; set; }
        public decimal Taxes15Percent { get; set; }
        public decimal Taxes18Percent { get; set; }
        public decimal Total { get; set; }
        public int? InventoryInputDestinationId { get; set; }
    }
}
