using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SMART.ERP.Application.Features.PriceListFeature.Commands.SetPriceListItemCommand;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.CompetitorScraper;
using SMART.ERP.Application.Services.PriceListResolver;
using SMART.ERP.Application.Specifications.RepricingSpecification;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;
using SMART.ERP.Domain.Settings;

namespace SMART.ERP.Application.Services.RepricingEngine
{
    public class RepricingEngineService : IRepricingEngineService
    {
        private readonly IRepositoryAsync<CompetitorSource> _sourceRepo;
        private readonly IReadRepositoryAsync<RepricingRule> _ruleRepo;
        private readonly IReadRepositoryAsync<RepricingSettings> _settingsRepo;
        private readonly IRepositoryAsync<PriceChangeLog> _logRepo;
        private readonly IReadRepositoryAsync<Product> _productRepo;
        private readonly IPriceListService _priceListService;
        private readonly ICompetitorScraperService _scraper;
        private readonly IMediator _mediator;
        private readonly RepricingScraperSettings _scraperSettings;
        private readonly ILogger<RepricingEngineService> _logger;

        public RepricingEngineService(
            IRepositoryAsync<CompetitorSource> sourceRepo,
            IReadRepositoryAsync<RepricingRule> ruleRepo,
            IReadRepositoryAsync<RepricingSettings> settingsRepo,
            IRepositoryAsync<PriceChangeLog> logRepo,
            IReadRepositoryAsync<Product> productRepo,
            IPriceListService priceListService,
            ICompetitorScraperService scraper,
            IMediator mediator,
            IOptions<RepricingScraperSettings> scraperSettings,
            ILogger<RepricingEngineService> logger)
        {
            _sourceRepo = sourceRepo;
            _ruleRepo = ruleRepo;
            _settingsRepo = settingsRepo;
            _logRepo = logRepo;
            _productRepo = productRepo;
            _priceListService = priceListService;
            _scraper = scraper;
            _mediator = mediator;
            _scraperSettings = scraperSettings.Value;
            _logger = logger;
        }

        public async Task<PriceChangeLog?> EvaluateAndApplyAsync(int productId, string actor, CancellationToken ct = default)
        {
            var product = await _productRepo.GetByIdAsync(productId, ct);
            if (product is null) return null;

            var sources = await _sourceRepo.ListAsync(
                new EnabledCompetitorSourcesByProductSpecification(productId), ct);
            if (sources.Count == 0) return null;

            var settings = await _settingsRepo.FirstOrDefaultAsync(
                new RepricingSettingsSingletonSpecification(), ct) ?? new RepricingSettings();

            // Regla efectiva (override del producto o defaults globales). Null = producto excluido.
            var rule = await ResolveEffectiveRuleAsync(productId, settings, ct);
            if (rule is null) return null;

            // 1) Leer cada fuente y persistir el último valor observado.
            var observations = new List<(CompetitorSource Source, decimal Comparable)>();
            for (var i = 0; i < sources.Count; i++)
            {
                var source = sources[i];
                var result = await _scraper.ScrapeAsync(source, ct);
                source.LastCheckedUtc = DateTime.UtcNow;

                if (result.Success && result.Price is > 0m)
                {
                    source.LastObservedPrice = result.Price;
                    source.LastObservedInStock = result.InStock;
                    source.LastError = null;

                    var inStock = result.InStock ?? true; // null = desconocido → se asume disponible
                    if (inStock)
                    {
                        var comparable = CompetitorPriceNormalizer.ToComparable(result.Price.Value, source.TaxBasis);
                        observations.Add((source, comparable));
                    }
                }
                else
                {
                    source.LastError = result.Error; // conservamos LastObservedPrice (último bueno)
                }

                await _sourceRepo.UpdateAsync(source, ct);

                // Cortesía entre peticiones de red (las fuentes manuales no hacen red).
                if (source.ParseStrategy != ParseStrategy.Manual && i < sources.Count - 1 && _scraperSettings.PolitenessDelayMs > 0)
                    await Task.Delay(_scraperSettings.PolitenessDelayMs, ct);
            }
            await _sourceRepo.SaveChangesAsync(ct);

            var defaultListId = await _priceListService.GetDefaultPriceListIdAsync(ct);
            var current = defaultListId.HasValue
                ? await _priceListService.GetPriceAsync(productId, defaultListId, ct) ?? 0m
                : 0m;

            // 2) Sin observaciones válidas → bitácora de fallo, sin cambio.
            if (observations.Count == 0)
            {
                return await PersistLogAsync(new PriceChangeLog
                {
                    ProductId = productId,
                    OldPrice = current,
                    ProposedPrice = current,
                    Status = PriceChangeStatus.ScrapeFailed,
                    Reason = "Ninguna fuente devolvió un precio válido y disponible.",
                    CreatedUtc = DateTime.UtcNow
                }, ct);
            }

            // 3) Calcular contra el competidor más bajo.
            var min = observations.OrderBy(o => o.Comparable).First();
            var outcome = RepricingCalculator.Calculate(new RepricingInput
            {
                MinCompetitorPrice = min.Comparable,
                CurrentPrice = current,
                CostPrice = product.CostPrice,
                Rule = rule
            });

            var log = new PriceChangeLog
            {
                ProductId = productId,
                CompetitorSourceIdMin = min.Source.Id,
                MinCompetitorPrice = min.Comparable,
                OldPrice = current,
                ProposedPrice = outcome.TargetPrice,
                FloorHit = outcome.FloorHit,
                Status = outcome.Status,
                Reason = outcome.Reason,
                CreatedUtc = DateTime.UtcNow
            };

            // 4) Aplicar si corresponde.
            if (outcome.ShouldWrite)
            {
                var autoApply = rule.AutoApply && settings.MonitoringEnabled;
                if (autoApply && defaultListId.HasValue)
                {
                    await _mediator.Send(new SetPriceListItemCommand
                    {
                        PriceListId = defaultListId.Value,
                        ProductId = productId,
                        Price = outcome.TargetPrice
                    }, ct);

                    log.Applied = true;
                    log.AppliedPrice = outcome.TargetPrice;
                    log.AppliedUtc = DateTime.UtcNow;
                    log.AppliedBy = actor;
                }
                else if (!defaultListId.HasValue)
                {
                    log.Status = PriceChangeStatus.Skipped;
                    log.Reason = "No hay lista de precios por defecto configurada.";
                }
                else
                {
                    log.Status = PriceChangeStatus.AwaitingApproval;
                }
            }

            return await PersistLogAsync(log, ct);
        }

        private async Task<EffectiveRepricingRule?> ResolveEffectiveRuleAsync(
            int productId, RepricingSettings settings, CancellationToken ct)
        {
            var ruleEntity = await _ruleRepo.FirstOrDefaultAsync(
                new RepricingRuleByProductSpecification(productId), ct);

            if (ruleEntity is not null)
            {
                if (!ruleEntity.IsEnabled) return null; // opt-out explícito del producto
                return new EffectiveRepricingRule
                {
                    UndercutMode = ruleEntity.UndercutMode,
                    UndercutValue = ruleEntity.UndercutValue,
                    MinMarginPercent = ruleEntity.MinMarginPercent,
                    RoundingMode = ruleEntity.RoundingMode,
                    MaxDecreasePercent = ruleEntity.MaxDecreasePercent,
                    MinChangeThreshold = ruleEntity.MinChangeThreshold,
                    AutoApply = ruleEntity.AutoApply
                };
            }

            return new EffectiveRepricingRule
            {
                UndercutMode = settings.DefaultUndercutMode,
                UndercutValue = settings.DefaultUndercutValue,
                MinMarginPercent = settings.DefaultMinMarginPercent,
                RoundingMode = settings.DefaultRoundingMode,
                MaxDecreasePercent = settings.DefaultMaxDecreasePercent,
                MinChangeThreshold = settings.DefaultMinChangeThreshold,
                AutoApply = settings.GlobalAutoApply
            };
        }

        private async Task<PriceChangeLog> PersistLogAsync(PriceChangeLog log, CancellationToken ct)
        {
            var saved = await _logRepo.AddAsync(log, ct);
            await _logRepo.SaveChangesAsync(ct);
            return saved;
        }
    }
}
