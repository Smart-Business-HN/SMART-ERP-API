namespace SMART.ERP.Application.DTOs.Provider
{
    public class ProviderPurchaseOrderLineDto
    {
        public int Id { get; set; }
        public string PurchaseOrderCode { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public DateTime DueDate { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public decimal Total { get; set; }
    }
}
