using MediatR;
using SMART.ERP.Application.DTOs.Repricing;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.RepricingSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.RepricingFeature.Queries
{
    public class GetRepricingSettingsQuery : IRequest<Response<RepricingSettingsDto>>
    {
        public class Handler : IRequestHandler<GetRepricingSettingsQuery, Response<RepricingSettingsDto>>
        {
            private readonly IReadRepositoryAsync<RepricingSettings> _repo;

            public Handler(IReadRepositoryAsync<RepricingSettings> repo) => _repo = repo;

            public async Task<Response<RepricingSettingsDto>> Handle(GetRepricingSettingsQuery request, CancellationToken ct)
            {
                var s = await _repo.FirstOrDefaultAsync(new RepricingSettingsSingletonSpecification(), ct)
                    ?? new RepricingSettings();

                return new Response<RepricingSettingsDto>(new RepricingSettingsDto
                {
                    Id = s.Id,
                    MonitoringEnabled = s.MonitoringEnabled,
                    GlobalAutoApply = s.GlobalAutoApply,
                    DefaultUndercutMode = s.DefaultUndercutMode,
                    DefaultUndercutValue = s.DefaultUndercutValue,
                    DefaultMinMarginPercent = s.DefaultMinMarginPercent,
                    DefaultRoundingMode = s.DefaultRoundingMode,
                    DefaultMaxDecreasePercent = s.DefaultMaxDecreasePercent,
                    DefaultMinChangeThreshold = s.DefaultMinChangeThreshold
                });
            }
        }
    }
}
