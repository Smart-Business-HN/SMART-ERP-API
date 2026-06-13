using MediatR;
using SMART.ERP.Application.DTOs.Repricing;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Features.PriceListFeature.Commands.SetPriceListItemCommand;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.PriceListResolver;
using SMART.ERP.Application.Services.RepricingEngine;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.RepricingFeature.Commands.ApplyProposedPriceCommand
{
    /// <summary>Aprueba y aplica una propuesta de precio pendiente (estado AwaitingApproval) a la lista por defecto.</summary>
    public class ApplyProposedPriceCommand : IRequest<Response<PriceChangeLogDto>>
    {
        public int PriceChangeLogId { get; set; }

        public class Handler : IRequestHandler<ApplyProposedPriceCommand, Response<PriceChangeLogDto>>
        {
            private readonly IRepositoryAsync<PriceChangeLog> _logRepo;
            private readonly IReadRepositoryAsync<Product> _productRepo;
            private readonly IPriceListService _priceListService;
            private readonly IMediator _mediator;
            private readonly IEcommerceRevalidationService _revalidation;

            public Handler(
                IRepositoryAsync<PriceChangeLog> logRepo,
                IReadRepositoryAsync<Product> productRepo,
                IPriceListService priceListService,
                IMediator mediator,
                IEcommerceRevalidationService revalidation)
            {
                _logRepo = logRepo;
                _productRepo = productRepo;
                _priceListService = priceListService;
                _mediator = mediator;
                _revalidation = revalidation;
            }

            public async Task<Response<PriceChangeLogDto>> Handle(ApplyProposedPriceCommand request, CancellationToken ct)
            {
                var log = await _logRepo.GetByIdAsync(request.PriceChangeLogId, ct)
                    ?? throw new KeyNotFoundException($"Registro de cambio {request.PriceChangeLogId} no encontrado");

                if (log.Applied)
                    return new Response<PriceChangeLogDto>("Esta propuesta ya fue aplicada.");

                if (log.ProposedPrice <= 0m)
                    throw new ApiException("El precio propuesto es inválido.");

                var product = await _productRepo.GetByIdAsync(log.ProductId, ct)
                    ?? throw new KeyNotFoundException($"Producto {log.ProductId} no encontrado");

                // Red de seguridad: nunca aplicar por debajo del costo.
                if (product.CostPrice > 0m && log.ProposedPrice < product.CostPrice)
                    throw new ApiException("El precio propuesto está por debajo del costo; no se aplica.");

                var defaultListId = await _priceListService.GetDefaultPriceListIdAsync(ct)
                    ?? throw new ApiException("No hay lista de precios por defecto configurada.");

                await _mediator.Send(new SetPriceListItemCommand
                {
                    PriceListId = defaultListId,
                    ProductId = log.ProductId,
                    Price = log.ProposedPrice
                }, ct);

                log.Applied = true;
                log.AppliedPrice = log.ProposedPrice;
                log.AppliedUtc = DateTime.UtcNow;
                log.AppliedBy = "admin";
                if (log.Status == PriceChangeStatus.AwaitingApproval)
                    log.Status = log.FloorHit ? PriceChangeStatus.FloorHeld : PriceChangeStatus.Applied;

                await _logRepo.UpdateAsync(log, ct);
                await _logRepo.SaveChangesAsync(ct);

                await _revalidation.RevalidateStoreAsync(ct);

                return new Response<PriceChangeLogDto>(log.ToDto(), "Precio aplicado correctamente");
            }
        }
    }
}
