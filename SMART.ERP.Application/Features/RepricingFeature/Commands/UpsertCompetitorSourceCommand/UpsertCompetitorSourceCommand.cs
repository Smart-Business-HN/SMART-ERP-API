using MediatR;
using SMART.ERP.Application.DTOs.Repricing;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.RepricingSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.RepricingFeature.Commands.UpsertCompetitorSourceCommand
{
    /// <summary>Crea o actualiza el mapeo de un producto a la página de un competidor.</summary>
    public class UpsertCompetitorSourceCommand : IRequest<Response<CompetitorSourceDto>>
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string CompetitorName { get; set; } = null!;
        public string ProductUrl { get; set; } = null!;
        public ParseStrategy ParseStrategy { get; set; } = ParseStrategy.Manual;
        public string? PriceSelector { get; set; }
        public bool IsEnabled { get; set; } = true;
        public CompetitorTaxBasis TaxBasis { get; set; } = CompetitorTaxBasis.IncludesIsv15;
        public string Currency { get; set; } = "HNL";
        /// <summary>Para fuentes manuales: precio que el admin ingresa a mano.</summary>
        public decimal? ManualPrice { get; set; }

        public class Handler : IRequestHandler<UpsertCompetitorSourceCommand, Response<CompetitorSourceDto>>
        {
            private readonly IRepositoryAsync<CompetitorSource> _repo;
            private readonly IReadRepositoryAsync<Product> _productRepo;

            public Handler(IRepositoryAsync<CompetitorSource> repo, IReadRepositoryAsync<Product> productRepo)
            {
                _repo = repo;
                _productRepo = productRepo;
            }

            public async Task<Response<CompetitorSourceDto>> Handle(UpsertCompetitorSourceCommand request, CancellationToken ct)
            {
                var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
                if (product is null)
                    throw new KeyNotFoundException($"Producto {request.ProductId} no encontrado");

                CompetitorSource entity;
                if (request.Id > 0)
                {
                    entity = await _repo.GetByIdAsync(request.Id, ct)
                        ?? throw new KeyNotFoundException($"Fuente de competencia {request.Id} no encontrada");
                }
                else
                {
                    var duplicate = await _repo.FirstOrDefaultAsync(
                        new CompetitorSourceByProductAndNameSpecification(request.ProductId, request.CompetitorName), ct);
                    if (duplicate is not null)
                        return new Response<CompetitorSourceDto>($"Ya existe una fuente '{request.CompetitorName}' para este producto.");

                    entity = new CompetitorSource
                    {
                        ProductId = request.ProductId,
                        CreationDate = DateTime.UtcNow,
                        CreatedBy = "admin"
                    };
                }

                entity.CompetitorName = request.CompetitorName;
                entity.ProductUrl = request.ProductUrl;
                entity.ParseStrategy = request.ParseStrategy;
                entity.PriceSelector = request.PriceSelector;
                entity.IsEnabled = request.IsEnabled;
                entity.TaxBasis = request.TaxBasis;
                entity.Currency = string.IsNullOrWhiteSpace(request.Currency) ? "HNL" : request.Currency;

                if (request.ParseStrategy == ParseStrategy.Manual && request.ManualPrice is > 0m)
                {
                    entity.LastObservedPrice = request.ManualPrice;
                    entity.LastObservedInStock = true;
                    entity.LastCheckedUtc = DateTime.UtcNow;
                    entity.LastError = null;
                }

                if (request.Id > 0)
                {
                    entity.ModificationDate = DateTime.UtcNow;
                    entity.ModificatedBy = "admin";
                    await _repo.UpdateAsync(entity, ct);
                }
                else
                {
                    entity = await _repo.AddAsync(entity, ct);
                }
                await _repo.SaveChangesAsync(ct);

                return new Response<CompetitorSourceDto>(Map(entity), "Fuente de competencia guardada correctamente");
            }

            private static CompetitorSourceDto Map(CompetitorSource e) => new()
            {
                Id = e.Id,
                ProductId = e.ProductId,
                CompetitorName = e.CompetitorName,
                ProductUrl = e.ProductUrl,
                ParseStrategy = e.ParseStrategy,
                PriceSelector = e.PriceSelector,
                IsEnabled = e.IsEnabled,
                TaxBasis = e.TaxBasis,
                Currency = e.Currency,
                LastCheckedUtc = e.LastCheckedUtc,
                LastObservedPrice = e.LastObservedPrice,
                LastObservedInStock = e.LastObservedInStock,
                LastError = e.LastError
            };
        }
    }
}
