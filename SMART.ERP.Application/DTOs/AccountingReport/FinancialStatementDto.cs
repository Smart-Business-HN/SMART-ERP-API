namespace SMART.ERP.Application.DTOs.AccountingReport
{
    /// <summary>Línea genérica de un estado financiero (agrupada por cuenta de mayor / tipo).</summary>
    public class FinancialStatementLineDto
    {
        public int LedgerAccountId { get; set; }
        public string AccountCode { get; set; } = null!;
        public string AccountName { get; set; } = null!;
        public decimal Amount { get; set; }
    }

    /// <summary>Estado de Situación Financiera (Balance General) a una fecha de corte.</summary>
    public class EstadoSituacionFinancieraDto
    {
        public DateTime CutoffDate { get; set; }
        public List<FinancialStatementLineDto> Activos { get; set; } = new();
        public List<FinancialStatementLineDto> Pasivos { get; set; } = new();
        public List<FinancialStatementLineDto> Patrimonio { get; set; } = new();
        public decimal TotalActivos { get; set; }
        public decimal TotalPasivos { get; set; }
        public decimal TotalPatrimonio { get; set; }
        /// <summary>Resultado del período aún no cerrado (Ingresos - Costos - Gastos), incluido en patrimonio.</summary>
        public decimal ResultadoDelPeriodo { get; set; }
        public bool IsBalanced { get; set; }
    }

    /// <summary>Estado de Resultados (Estado de Resultado Integral) para un rango de fechas.</summary>
    public class EstadoResultadosDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<FinancialStatementLineDto> Ingresos { get; set; } = new();
        public List<FinancialStatementLineDto> Costos { get; set; } = new();
        public List<FinancialStatementLineDto> Gastos { get; set; } = new();
        public decimal TotalIngresos { get; set; }
        public decimal TotalCostos { get; set; }
        public decimal TotalGastos { get; set; }
        public decimal UtilidadBruta { get; set; }
        public decimal UtilidadNeta { get; set; }
    }
}
