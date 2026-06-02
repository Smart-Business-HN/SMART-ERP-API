namespace SMART.ERP.Application.DTOs.AdvisorGoal
{
    public class AdvisorCommissionDto
    {
        public Guid UserId { get; set; }
        public string? FullName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal NetSales { get; set; }
        public decimal? CommissionPercentage { get; set; }
        public decimal? CommissionAmount { get; set; }
    }
}
