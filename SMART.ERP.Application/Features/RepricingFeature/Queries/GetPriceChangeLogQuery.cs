using MediatR;
using SMART.ERP.Application.DTOs.Repricing;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.RepricingSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.RepricingFeature.Queries
{
    public class GetPriceChangeLogQuery : IRequest<PagedResponse<List<PriceChangeLogDto>>>
    {
        public int? ProductId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 20;

        public class Handler : IRequestHandler<GetPriceChangeLogQuery, PagedResponse<List<PriceChangeLogDto>>>
        {
            private readonly IReadRepositoryAsync<PriceChangeLog> _repo;

            public Handler(IReadRepositoryAsync<PriceChangeLog> repo) => _repo = repo;

            public async Task<PagedResponse<List<PriceChangeLogDto>>> Handle(GetPriceChangeLogQuery request, CancellationToken ct)
            {
                var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;

                var items = await _repo.ListAsync(
                    new PriceChangeLogPagedSpecification(request.ProductId, request.PageNumber, pageSize), ct);
                var total = await _repo.CountAsync(
                    new CountPriceChangeLogSpecification(request.ProductId), ct);

                var dtos = items.Select(x => x.ToDto()).ToList();
                return new PagedResponse<List<PriceChangeLogDto>>(dtos, request.PageNumber, pageSize, total);
            }
        }
    }
}
