using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.DTOs.ProductToPurchase;
using SMART.ERP.Application.DTOs.Provider;
using SMART.ERP.Application.DTOs.Status;
using SMART.ERP.Application.DTOs.User;

namespace SMART.ERP.Application.DTOs.PurchaseOrder
{
    public class PurchaseOrderDto
    {
        public int Id { get; init; }
        public int ProviderId { get; set; }
        public virtual ProviderDto? Provider { get; set; }
        public string PurchaseOrderCode { get; set; } = null!;
        public int BranchOfficeId { get; set; }
        public virtual BranchOfficeDto? BranchOffice { get; set; }
        public Guid UserId { get; set; }
        public virtual UserDto? User { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? Observations { get; set; }
        public string? TermsAndConditions { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public int StatusId { get; set; }
        public virtual StatusDto? Status { get; set; }
        public int PrefixId { get; set; }
        public virtual PrefixDto? Prefix { get; set; }
        public int? PurchaseBillDestinationId { get; set; }
        public List<ProductToPurchaseDto>? ProductsToPurchase { get; set; }
    }
}
