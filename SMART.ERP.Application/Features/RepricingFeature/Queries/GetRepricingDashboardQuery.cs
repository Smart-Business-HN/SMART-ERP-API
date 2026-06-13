using MediatR;
using SMART.ERP.Application.DTOs.Repricing;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.PriceListResolver;
using SMART.ERP.Application.Services.RepricingEngine;
using SMART.ERP.Application.Specifications.RepricingSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.RepricingFeature.Queries
{
    /// <summary>Resumen por producto monitoreado: precio actual, mínimo de competencia y último cambio.</summary>
    public class GetRepricingDashboardQuery : IRequest<Response<List<RepricingDashboardItemDto>>>
    {
        public class Handler : IRequestHandler<GetRepricingDashboardQuery, Response<List<RepricingDashboardItemDto>>>
        {
            private readonly IReadRepositoryAsync<CompetitorSource> _sourceRepo;
            private readonly IReadRepositoryAsync<RepricingRule> _ruleRepo;
            private readonly IReadRepositoryAsync<RepricingSettings> _settingsRepo;
            private readonly IReadRepositoryAsync<PriceChangeLog> _logRepo;
            private readonly IPriceListService _priceListService;

            public Handler(
                IReadRepositoryAsync<CompetitorSource> sourceRepo,
                IReadRepositoryAsync<RepricingRule> ruleRepo,
                IReadRepositoryAsync<RepricingSettings> settingsRepo,
                IReadRepositoryAsync<PriceChangeLog> logRepo,
                IPriceListService priceListService)
            {
                _sourceRepo = sourceRepo;
                _ruleRepo = ruleRepo;
                _settingsRepo = settingsRepo;
                _logRepo = logRepo;
                _priceListService = priceListService;
            }

            public async Task<Response<List<RepricingDashboardItemDto>>> Handle(GetRepricingDashboardQuery request, CancellationToken ct)
            {
                var sources = await _sourceRepo.ListAsync(
                    new AllEnabledCompetitorSourcesWithProductSpecification(), ct);

                var byProduct = sources
                    .Where(s => s.Product is not null)
                    .GroupBy(s => s.ProductId)
                    .ToList();

                var result = new List<RepricingDashboardItemDto>();
                if (byProduct.Count == 0)
                    return new Response<List<RepricingDashboardItemDto>>(result);

                var productIds = byProduct.Select(g => g.Key).ToList();

                var settings = await _settingsRepo.FirstOrDefaultAsync(new RepricingSettingsSingletonSpecification(), ct)
                    ?? new RepricingSettings();
                var rules = (await _ruleRepo.ListAsync(new RepricingRulesByProductsSpecification(productIds), ct))
                    .ToDictionary(r => r.ProductId, r => r);

                var defaultListId = await _priceListService.GetDefaultPriceListIdAsync(ct);
                var currentPrices = defaultListId.HasValue
                    ? await _priceListService.GetPricesAsync(productIds, defaultListId, ct)
                    : new Dictionary<int, decimal>();

                foreach (var group in byProduct)
                {
                    var product = group.First().Product!;

                    // Mínimo de competencia observado (normalizado, solo disponibles).
                    var observed = group
                        .Where(s => s.LastObservedPrice is > 0m && (s.LastObservedInStock ?? true))
                        .Select(s => CompetitorPriceNormalizer.ToComparable(s.LastObservedPrice!.Value, s.TaxBasis))
                        .ToList();

                    var latest = await _logRepo.FirstOrDefaultAsync(
                        new LatestPriceChangeLogByProductSpecification(group.Key), ct);

                    rules.TryGetValue(group.Key, out var rule);

                    result.Add(new RepricingDashboardItemDto
                    {
                        ProductId = group.Key,
                        ProductCode = product.Code,
                        ProductName = product.Name,
                        CurrentPrice = currentPrices.TryGetValue(group.Key, out var price) ? price : 0m,
                        CostPrice = product.CostPrice,
                        MinCompetitorPrice = observed.Count > 0 ? observed.Min() : null,
                        LastAppliedPrice = latest?.AppliedPrice,
                        LastEvaluatedUtc = latest?.CreatedUtc,
                        LastFloorHit = latest?.FloorHit ?? false,
                        LastStatus = latest?.Status,
                        EnabledSourceCount = group.Count(),
                        AutoApply = rule is not null ? rule.AutoApply : settings.GlobalAutoApply
                    });
                }

                return new Response<List<RepricingDashboardItemDto>>(
                    result.OrderBy(r => r.ProductName).ToList());
            }
        }
    }
}
