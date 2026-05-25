namespace SMART.ERP.Application.DTOs.AccountingReport
{
    /// <summary>Balance de Comprobación (Trial Balance) para un rango de fechas.</summary>
    public class BalanceComprobacionDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<BalanceComprobacionLineDto> Lines { get; set; } = new();
        public decimal TotalDebe { get; set; }
        public decimal TotalHaber { get; set; }
        public decimal TotalSaldoDeudor { get; set; }
        public decimal TotalSaldoAcreedor { get; set; }
        public bool IsBalanced { get; set; }
    }

    public class BalanceComprobacionLineDto
    {
        public int LedgerAccountId { get; set; }
        public string AccountCode { get; set; } = null!;
        public string AccountName { get; set; } = null!;
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
        public decimal SaldoDeudor { get; set; }
        public decimal SaldoAcreedor { get; set; }
    }
}
