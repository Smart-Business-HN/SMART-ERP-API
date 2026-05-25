using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.DTOs.FiscalPeriod
{
    public class FiscalPeriodDto
    {
        public int Id { get; set; }
        public int FiscalYearId { get; set; }
        public int PeriodNumber { get; set; }
        public string Name { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public FiscalPeriodStatus Status { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string? ClosedBy { get; set; }
    }
}
