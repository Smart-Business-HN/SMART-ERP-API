namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class AccountsPayableAgingDto
    {
        public decimal Current { get; set; }
        public decimal Days31To60 { get; set; }
        public decimal Days61To90 { get; set; }
        public decimal Over90Days { get; set; }
        public decimal Total { get; set; }
        public List<AgingDetailDto> Details { get; set; } = new();
    }
}
