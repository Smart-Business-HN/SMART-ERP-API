using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.DTOs.PurchaseOrder;

namespace SMART.ERP.Application.DTOs.ProductToPurchase
{
    public class ProductToPurchaseDto
    {
        public int Id { get; init; }
        public int PurchaseOrderId { get; set; }
        public virtual PurchaseOrderDto? PurchaseOrder { get; set; }
        public int? ProductId { get; set; }
        public virtual ProductDto? Product { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public int TaxId { get; set; }
        public virtual TaxDto? Tax { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalLine { get; set; }
    }
}
