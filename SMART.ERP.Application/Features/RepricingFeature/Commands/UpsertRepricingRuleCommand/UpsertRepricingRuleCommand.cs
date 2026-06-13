using MediatR;
using SMART.ERP.Application.DTOs.Repricing;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.RepricingSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.RepricingFeature.Commands.UpsertRepricingRuleCommand
{
    /// <summary>Crea o actualiza la regla de re-fijación (override) de un producto.</summary>
    public class UpsertRepricingRuleCommand : IRequest<Response<RepricingRuleDto>>
    {
        public int ProductId { get; set; }
        public UndercutMode UndercutMode { get; set; } = UndercutMode.Percent;
        public decimal UndercutValue { get; set; } = 0.01m;
        public decimal MinMarginPercent { get; set; } = 0.15m;
        public PriceRoundingMode RoundingMode { get; set; } = PriceRoundingMode.None;
        public decimal MaxDecreasePercent { get; set; }
        public decimal MinChangeThreshold { get; set; }
        public bool AutoApply { get; set; } = true;
        public bool IsEnabled { get; set; } = true;

        public class Handler : IRequestHandler<UpsertRepricingRuleCommand, Response<RepricingRuleDto>>
        {
            private readonly IRepositoryAsync<RepricingRule> _repo;
            private readonly IReadRepositoryAsync<Product> _productRepo;

            public Handler(IRepositoryAsync<RepricingRule> repo, IReadRepositoryAsync<Product> productRepo)
            {
                _repo = repo;
                _productRepo = productRepo;
            }

            public async Task<Response<RepricingRuleDto>> Handle(UpsertRepricingRuleCommand request, CancellationToken ct)
            {
                var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
                if (product is null)
                    throw new KeyNotFoundException($"Producto {request.ProductId} no encontrado");

                var entity = await _repo.FirstOrDefaultAsync(
                    new RepricingRuleByProductSpecification(request.ProductId), ct);

                var isNew = entity is null;
                entity ??= new RepricingRule
                {
                    ProductId = request.ProductId,
                    CreationDate = DateTime.UtcNow,
                    CreatedBy = "admin"
                };

                entity.UndercutMode = request.UndercutMode;
                entity.UndercutValue = request.UndercutValue;
                entity.MinMarginPercent = request.MinMarginPercent;
                entity.RoundingMode = request.RoundingMode;
                entity.MaxDecreasePercent = request.MaxDecreasePercent;
                entity.MinChangeThreshold = request.MinChangeThreshold;
                entity.AutoApply = request.AutoApply;
                entity.IsEnabled = request.IsEnabled;

                if (isNew)
                {
                    entity = await _repo.AddAsync(entity, ct);
                }
                else
                {
                    entity.ModificationDate = DateTime.UtcNow;
                    entity.ModificatedBy = "admin";
                    await _repo.UpdateAsync(entity, ct);
                }
                await _repo.SaveChangesAsync(ct);

                return new Response<RepricingRuleDto>(new RepricingRuleDto
                {
                    Id = entity.Id,
                    ProductId = entity.ProductId,
                    UndercutMode = entity.UndercutMode,
                    UndercutValue = entity.UndercutValue,
                    MinMarginPercent = entity.MinMarginPercent,
                    RoundingMode = entity.RoundingMode,
                    MaxDecreasePercent = entity.MaxDecreasePercent,
                    MinChangeThreshold = entity.MinChangeThreshold,
                    AutoApply = entity.AutoApply,
                    IsEnabled = entity.IsEnabled,
                    IsGlobalDefault = false
                }, "Regla de re-fijación guardada correctamente");
            }
        }
    }
}
