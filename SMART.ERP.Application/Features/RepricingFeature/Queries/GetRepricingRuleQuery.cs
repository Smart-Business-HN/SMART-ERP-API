using MediatR;
using SMART.ERP.Application.DTOs.Repricing;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.RepricingSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.RepricingFeature.Queries
{
    /// <summary>
    /// Devuelve la regla efectiva de un producto: su override si existe, o los defaults globales
    /// (marcados con <see cref="RepricingRuleDto.IsGlobalDefault"/>).
    /// </summary>
    public class GetRepricingRuleQuery : IRequest<Response<RepricingRuleDto>>
    {
        public int ProductId { get; set; }

        public class Handler : IRequestHandler<GetRepricingRuleQuery, Response<RepricingRuleDto>>
        {
            private readonly IReadRepositoryAsync<RepricingRule> _ruleRepo;
            private readonly IReadRepositoryAsync<RepricingSettings> _settingsRepo;

            public Handler(
                IReadRepositoryAsync<RepricingRule> ruleRepo,
                IReadRepositoryAsync<RepricingSettings> settingsRepo)
            {
                _ruleRepo = ruleRepo;
                _settingsRepo = settingsRepo;
            }

            public async Task<Response<RepricingRuleDto>> Handle(GetRepricingRuleQuery request, CancellationToken ct)
            {
                var rule = await _ruleRepo.FirstOrDefaultAsync(
                    new RepricingRuleByProductSpecification(request.ProductId), ct);

                if (rule is not null)
                {
                    return new Response<RepricingRuleDto>(new RepricingRuleDto
                    {
                        Id = rule.Id,
                        ProductId = rule.ProductId,
                        UndercutMode = rule.UndercutMode,
                        UndercutValue = rule.UndercutValue,
                        MinMarginPercent = rule.MinMarginPercent,
                        RoundingMode = rule.RoundingMode,
                        MaxDecreasePercent = rule.MaxDecreasePercent,
                        MinChangeThreshold = rule.MinChangeThreshold,
                        AutoApply = rule.AutoApply,
                        IsEnabled = rule.IsEnabled,
                        IsGlobalDefault = false
                    });
                }

                var settings = await _settingsRepo.FirstOrDefaultAsync(new RepricingSettingsSingletonSpecification(), ct)
                    ?? new RepricingSettings();

                return new Response<RepricingRuleDto>(new RepricingRuleDto
                {
                    Id = null,
                    ProductId = request.ProductId,
                    UndercutMode = settings.DefaultUndercutMode,
                    UndercutValue = settings.DefaultUndercutValue,
                    MinMarginPercent = settings.DefaultMinMarginPercent,
                    RoundingMode = settings.DefaultRoundingMode,
                    MaxDecreasePercent = settings.DefaultMaxDecreasePercent,
                    MinChangeThreshold = settings.DefaultMinChangeThreshold,
                    AutoApply = settings.GlobalAutoApply,
                    IsEnabled = true,
                    IsGlobalDefault = true
                });
            }
        }
    }
}
