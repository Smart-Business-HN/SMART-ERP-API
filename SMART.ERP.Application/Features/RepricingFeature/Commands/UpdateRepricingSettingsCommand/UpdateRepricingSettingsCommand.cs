using MediatR;
using SMART.ERP.Application.DTOs.Repricing;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.RepricingSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.RepricingFeature.Commands.UpdateRepricingSettingsCommand
{
    /// <summary>Actualiza la configuración global de re-fijación (fila única).</summary>
    public class UpdateRepricingSettingsCommand : IRequest<Response<RepricingSettingsDto>>
    {
        public bool MonitoringEnabled { get; set; }
        public bool GlobalAutoApply { get; set; }
        public UndercutMode DefaultUndercutMode { get; set; } = UndercutMode.Percent;
        public decimal DefaultUndercutValue { get; set; } = 0.01m;
        public decimal DefaultMinMarginPercent { get; set; } = 0.15m;
        public PriceRoundingMode DefaultRoundingMode { get; set; } = PriceRoundingMode.None;
        public decimal DefaultMaxDecreasePercent { get; set; }
        public decimal DefaultMinChangeThreshold { get; set; }

        public class Handler : IRequestHandler<UpdateRepricingSettingsCommand, Response<RepricingSettingsDto>>
        {
            private readonly IRepositoryAsync<RepricingSettings> _repo;

            public Handler(IRepositoryAsync<RepricingSettings> repo) => _repo = repo;

            public async Task<Response<RepricingSettingsDto>> Handle(UpdateRepricingSettingsCommand request, CancellationToken ct)
            {
                var entity = await _repo.FirstOrDefaultAsync(new RepricingSettingsSingletonSpecification(), ct);
                var isNew = entity is null;
                entity ??= new RepricingSettings();

                entity.MonitoringEnabled = request.MonitoringEnabled;
                entity.GlobalAutoApply = request.GlobalAutoApply;
                entity.DefaultUndercutMode = request.DefaultUndercutMode;
                entity.DefaultUndercutValue = request.DefaultUndercutValue;
                entity.DefaultMinMarginPercent = request.DefaultMinMarginPercent;
                entity.DefaultRoundingMode = request.DefaultRoundingMode;
                entity.DefaultMaxDecreasePercent = request.DefaultMaxDecreasePercent;
                entity.DefaultMinChangeThreshold = request.DefaultMinChangeThreshold;
                entity.ModificationDate = DateTime.UtcNow;
                entity.ModifiedBy = "admin";

                if (isNew)
                    entity = await _repo.AddAsync(entity, ct);
                else
                    await _repo.UpdateAsync(entity, ct);
                await _repo.SaveChangesAsync(ct);

                return new Response<RepricingSettingsDto>(new RepricingSettingsDto
                {
                    Id = entity.Id,
                    MonitoringEnabled = entity.MonitoringEnabled,
                    GlobalAutoApply = entity.GlobalAutoApply,
                    DefaultUndercutMode = entity.DefaultUndercutMode,
                    DefaultUndercutValue = entity.DefaultUndercutValue,
                    DefaultMinMarginPercent = entity.DefaultMinMarginPercent,
                    DefaultRoundingMode = entity.DefaultRoundingMode,
                    DefaultMaxDecreasePercent = entity.DefaultMaxDecreasePercent,
                    DefaultMinChangeThreshold = entity.DefaultMinChangeThreshold
                }, "Configuración de re-fijación guardada correctamente");
            }
        }
    }
}
