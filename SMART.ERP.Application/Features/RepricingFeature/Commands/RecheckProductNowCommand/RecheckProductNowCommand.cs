using MediatR;
using SMART.ERP.Application.DTOs.Repricing;
using SMART.ERP.Application.Services.RepricingEngine;
using SMART.ERP.Application.Wrappers;

namespace SMART.ERP.Application.Features.RepricingFeature.Commands.RecheckProductNowCommand
{
    /// <summary>Re-evalúa un producto ahora mismo: scrapea sus fuentes, calcula y (si aplica) re-fija el precio.</summary>
    public class RecheckProductNowCommand : IRequest<Response<PriceChangeLogDto>>
    {
        public int ProductId { get; set; }

        public class Handler : IRequestHandler<RecheckProductNowCommand, Response<PriceChangeLogDto>>
        {
            private readonly IRepricingEngineService _engine;
            private readonly IEcommerceRevalidationService _revalidation;

            public Handler(IRepricingEngineService engine, IEcommerceRevalidationService revalidation)
            {
                _engine = engine;
                _revalidation = revalidation;
            }

            public async Task<Response<PriceChangeLogDto>> Handle(RecheckProductNowCommand request, CancellationToken ct)
            {
                var log = await _engine.EvaluateAndApplyAsync(request.ProductId, "admin", ct);
                if (log is null)
                    return new Response<PriceChangeLogDto>("El producto no tiene fuentes de competencia habilitadas o está excluido.");

                if (log.Applied)
                    await _revalidation.RevalidateStoreAsync(ct);

                return new Response<PriceChangeLogDto>(log.ToDto(),
                    log.Applied ? "Precio re-evaluado y aplicado" : "Precio re-evaluado");
            }
        }
    }
}
