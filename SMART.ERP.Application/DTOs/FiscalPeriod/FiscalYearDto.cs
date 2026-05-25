using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.DTOs.FiscalPeriod
{
    public class FiscalYearDto
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string Name { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public FiscalPeriodStatus Status { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string? ClosedBy { get; set; }
        public int? ClosingJournalEntryId { get; set; }
        public List<FiscalPeriodDto> Periods { get; set; } = new();
    }
}
