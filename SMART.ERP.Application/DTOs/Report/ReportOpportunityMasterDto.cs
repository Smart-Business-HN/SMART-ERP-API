namespace SMART.ERP.Application.DTOs.Report
{
    public class ReportOpportunityMasterDto
    {
        public DateTime CreationDate { get; set; }
        public DateTime? ClosingDate { get; set; }
        public string Code { get; set; } = null!;
        public string Customer { get; set; } = null!;
        public string? Department { get; set; }
        public string User { get; set; } = null!;
        public string Step { get; set; } = null!;
        public decimal CustomerBudget { get; set; }
        public string? Products { get; set; }
        public string? WinReason { get; set; }
        public string? LossReason { get; set; }
        public int QtyItems { get; set; }
        public decimal Total { get; set; }
    }
}
