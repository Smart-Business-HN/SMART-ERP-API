using MediatR;
using SMART.ERP.Application.DTOs.Repricing;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.RepricingSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.RepricingFeature.Queries
{
    public class GetCompetitorSourcesByProductQuery : IRequest<Response<List<CompetitorSourceDto>>>
    {
        public int ProductId { get; set; }

        public class Handler : IRequestHandler<GetCompetitorSourcesByProductQuery, Response<List<CompetitorSourceDto>>>
        {
            private readonly IReadRepositoryAsync<CompetitorSource> _repo;

            public Handler(IReadRepositoryAsync<CompetitorSource> repo) => _repo = repo;

            public async Task<Response<List<CompetitorSourceDto>>> Handle(GetCompetitorSourcesByProductQuery request, CancellationToken ct)
            {
                var sources = await _repo.ListAsync(
                    new CompetitorSourcesByProductSpecification(request.ProductId), ct);

                var dtos = sources.Select(e => new CompetitorSourceDto
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
                }).ToList();

                return new Response<List<CompetitorSourceDto>>(dtos);
            }
        }
    }
}
