namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class TopSuppliersOutstandingDto
    {
        public List<SupplierOutstandingDto> Suppliers { get; set; } = new();
    }

    public class SupplierOutstandingDto
    {
        public int ProviderId { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public decimal TotalOutstanding { get; set; }
        public int BillCount { get; set; }
        public DateTime? OldestBillDate { get; set; }
    }
}
