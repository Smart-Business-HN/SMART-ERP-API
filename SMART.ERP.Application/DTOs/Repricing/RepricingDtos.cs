using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.DTOs.Repricing
{
    public class CompetitorSourceDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string CompetitorName { get; set; } = null!;
        public string ProductUrl { get; set; } = null!;
        public ParseStrategy ParseStrategy { get; set; }
        public string? PriceSelector { get; set; }
        public bool IsEnabled { get; set; }
        public CompetitorTaxBasis TaxBasis { get; set; }
        public string Currency { get; set; } = "HNL";
        public DateTime? LastCheckedUtc { get; set; }
        public decimal? LastObservedPrice { get; set; }
        public bool? LastObservedInStock { get; set; }
        public string? LastError { get; set; }
    }

    public class RepricingRuleDto
    {
        public int? Id { get; set; }
        public int ProductId { get; set; }
        public UndercutMode UndercutMode { get; set; }
        public decimal UndercutValue { get; set; }
        public decimal MinMarginPercent { get; set; }
        public PriceRoundingMode RoundingMode { get; set; }
        public decimal MaxDecreasePercent { get; set; }
        public decimal MinChangeThreshold { get; set; }
        public bool AutoApply { get; set; }
        public bool IsEnabled { get; set; }
        /// <summary>True si estos valores vienen de los defaults globales (el producto no tiene regla propia).</summary>
        public bool IsGlobalDefault { get; set; }
    }

    public class RepricingSettingsDto
    {
        public int Id { get; set; }
        public bool MonitoringEnabled { get; set; }
        public bool GlobalAutoApply { get; set; }
        public UndercutMode DefaultUndercutMode { get; set; }
        public decimal DefaultUndercutValue { get; set; }
        public decimal DefaultMinMarginPercent { get; set; }
        public PriceRoundingMode DefaultRoundingMode { get; set; }
        public decimal DefaultMaxDecreasePercent { get; set; }
        public decimal DefaultMinChangeThreshold { get; set; }
    }

    public class PriceChangeLogDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? CompetitorSourceIdMin { get; set; }
        public decimal? MinCompetitorPrice { get; set; }
        public decimal OldPrice { get; set; }
        public decimal ProposedPrice { get; set; }
        public decimal? AppliedPrice { get; set; }
        public bool FloorHit { get; set; }
        public bool Applied { get; set; }
        public PriceChangeStatus Status { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime? AppliedUtc { get; set; }
        public string? AppliedBy { get; set; }
    }

    /// <summary>Resumen por producto para la lista/dashboard de re-fijación.</summary>
    public class RepricingDashboardItemDto
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public decimal CurrentPrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal? MinCompetitorPrice { get; set; }
        public decimal? LastAppliedPrice { get; set; }
        public DateTime? LastEvaluatedUtc { get; set; }
        public bool LastFloorHit { get; set; }
        public PriceChangeStatus? LastStatus { get; set; }
        public int EnabledSourceCount { get; set; }
        public bool AutoApply { get; set; }
    }
}
